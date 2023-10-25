import axios from 'axios';
import fs from 'fs';
import ora from 'ora';

import { config } from './config.js';
import { client } from './graphClient.js';

// update to match your tenant
const DB2HOST = "";
const IBM_TENANT_ID = "";
const DB2_USER = "";
const DB2_password = "";
const ACCESS_TOKEN_API = "/dbapi/v4/auth/tokens";
const EXEC_SQL_JOB = "/dbapi/v4/sql_jobs";

const errorHandler = (functionName, error) => {
  console.error(`[${functionName}]:error`, error);
};

const getDB2AccessToken = async () => {
  const options = {
    method: "post",
    headers: {
      "content-type": "application/json",
      "x-deployment-id": IBM_TENANT_ID,
    },
    url: `https://${DB2HOST}${ACCESS_TOKEN_API}`,
    data: {
      userid: DB2_USER,
      password: DB2_password,
    },
  };

  try {
    const res = await axios(options);
    return res.data.token;
  } catch (error) {
    errorHandler("getDB2AccessToken", error);
  }
};

const createJSONFromData = (rows, columns) => {
  if (!rows || !columns) return;
  const items = [];
  const jsonObject = [];
  for (let index = 0; index < rows.length; index++) {
    const row = rows[index];
    const jsonObject = {};
    for (let i = 0; i < columns.length; i++) {
      jsonObject[columns[i]] = row[i];
    }
    const jsonString = JSON.stringify(jsonObject);
    items.push(jsonString);
  }

  return items;
};

const checkJobExecution = async (jobId) => {
  const accessToken = await getDB2AccessToken();
  try {
    const options = {
      method: "get",
      headers: {
        "content-type": "application/json",
        "x-deployment-id": IBM_TENANT_ID,
        authorization: `Bearer ${accessToken}`,
      },
      url: `https://${DB2HOST}${EXEC_SQL_JOB}/${jobId}`,
    };
    const res = await axios(options);
    return res.data;
  } catch (error) {
    errorHandler("checkJobExecution", error);
  }
};

const submitDb2Job = async (query) => {
  try {
    if (!query) {
      console.log("[getData ] error:", "query to execute is undefined");
      return;
    }
    const accessToken = await getDB2AccessToken();
    const queryDef = {
      commands: query,
      limit: 1000,
      separator: ";",
      stop_on_error: "no",
    };
    const options = {
      method: "post",
      headers: {
        "content-type": "application/json",
        "x-deployment-id": IBM_TENANT_ID,
        authorization: `Bearer ${accessToken}`,
      },
      url: `https://${DB2HOST}${EXEC_SQL_JOB}`,
      data: queryDef,
    };

    const res = await axios(options);
    return res.data;
  } catch (error) {
    errorHandler("submitDb2Job", error);
  }
};
const getData = async () => {
  const sql =
    "select * from CUST_ORD left join DWH00649.CUST C2 on C2.CUST_CODE = CUST_ORD.CUST_CODE left join DWH00649.CUST_ORD_DETL COD on CUST_ORD.ORD_NBR = COD.ORD_NBR left join DWH00649.PRODUCT P on P.PRODUCT_NUMBER = COD.PROD_NBR left  join DWH00649.PRODUCT_NAME_LOOKUP PNL on P.PRODUCT_NUMBER = PNL.PRODUCT_NUMBER order by   C2.CUST_CODE";
  try {
    const jobInfo = await submitDb2Job(sql);
    if (jobInfo) {
      const { id } = jobInfo;
      const interval = setInterval(async () => {
        const jobdata = await checkJobExecution(id);
        const { status, results } = jobdata || {};
        const { columns, rows } = results.length ? results[0] : {};
        if (status === "completed" && !jobdata.results.length) {
          clearInterval(interval);
          return;
        }
        switch (status) {
          case "completed":
            clearInterval(interval);
            const fmtdata = createJSONFromData(rows, columns);
            addDataToConnector(fmtdata);
            break;
          case "failed":
            errorHandler("DB2JobExecutuion", new Error(results[0]?.error));
            clearInterval(interval);
            break;
        }
      }, 500);
    }
  } catch (error) {
    errorHandler("getData", error);
    return undefined;
    throw error;
  }
};

const getUniqueItems = (array, property, propertytofilter, filter) => {
  const propertyToCheck = property;
  const seen = {};
  const uniqueArray = array.filter((obj) => {
    if (!seen[obj[propertyToCheck]]) {
      seen[obj[propertyToCheck]] = true;
      return true;
    }
    return false;
  });
  if (propertytofilter) {
    return uniqueArray.filter((value) => value[propertytofilter] === filter);
  } else {
    return uniqueArray;
  }
};

const mapData = (clientData, orders, orderDetails) => {
  return {
    id: clientData.CUST_CODE,
    properties: {
      custcode: clientData.CUST_CODE,
      custname: `${clientData.CUST_FRST_NAME} ${clientData.CUST_LAST_NAME}`,
      "orders@odata.type": "Collection(String)",
      orders: orders.map((order) => order.ORD_NBR),
      "orderdates@odata.type": "Collection(String)",
      orderdates: orders.map((order) => order.ORD_DATE),
      "ordertotals@odata.type": "Collection(String)",
      ordertotals: orders.map((order) => order.ORD_TOT_COST),
      "orderstatus@odata.type": "Collection(String)",
      orderstatus: orders.map((order) => order.ORD_STAT),
      country: clientData.CUST_CTRY_CODE,
      city: clientData.CUST_CITY,
      state: clientData.CUST_PROV_STATE,
      email: clientData.CUST_EMAIL,
      phone: clientData.CUST_PHN_NBR,
      address: `${clientData.CUST_ADDR1} ${clientData.CUST_POST_ZONE}`,
    },
    content: {
      value: `${clientData.CUST_CODE}-${clientData.CUST_FRST_NAME} ${clientData.CUST_LAST_NAME}`,
      type: "text",
    },
    acl: [
      {
        accessType: "grant",
        type: "everyone",
        value: "everyone",
      },
    ],
  };
};

const pushItem = (clientData, mappedData, callback, errors) => {
  const { connection } = config;
  client
    .api(`/external/connections/${connection.id}/items/${clientData.CUST_CODE}`)
    .header("content-type", "application/json")
    .put(mappedData)
    .then(() => {})
    .catch((error) => {
      errors.push({
        mappedData,
        error,
      });
    })
    .finally(() => callback());
};

const addDataToConnector = async (data) => {
  const deserializedArray = data.map((serializedObject) => JSON.parse(serializedObject));
  const clientData = getUniqueItems(deserializedArray, "CUST_CODE");
  let orders = [];
  let orderDetails = [];
  let numclientData = clientData.length;
  let running = 0;
  const errors = [];
  let completed = 0;
  const spinner = ora("Loading content...").start();

  const printStatus = () => {
    spinner.text = `Total: ${numclientData} | Processed: ${completed} (${(
      (completed / numclientData) *
      100
    ).toFixed(1)}%)   | Errors: ${errors.length} | Active: ${ running}...`;
  };

  for (const item of clientData) {
    orders = getUniqueItems(deserializedArray, "ORD_NBR", "CUST_CODE", item.CUST_CODE);
    for (const order of orders) {
      orderDetails = getUniqueItems(deserializedArray, "ORD_DETL_CODE", "ORD_NBR", order.ORD_NBR);
    }
    printStatus();
    const mappedData = mapData(item, orders, orderDetails);

    running++;
    pushItem(
      item,
      mappedData,
      () => {
        running--;
        completed++;
      },
      errors
    );
  }
  while (running > 0) {
    await new Promise((resolve) => setTimeout(resolve, 100));
    printStatus();
  }
  if (errors.length > 0) {
    const errorFileName = `errors-${new Date().toISOString().replace(/[^\d\w]/g, "-")}.json`;
    fs.writeFileSync(errorFileName, JSON.stringify(errors, null, 2));
    spinner.warn(`Completed with ${errors.length} errors. See ${errorFileName} for details.`);
  } else {
    spinner.succeed();
  }
};
const main = () => {
  getData();
};

main();
