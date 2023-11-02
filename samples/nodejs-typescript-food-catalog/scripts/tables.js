const { TableClient, TableServiceClient } = require("@azure/data-tables");
const { table } = require("console");
const fs = require("fs");
const path = require("path");

(async () => {
    const connectionString = process.argv[2] ? process.argv[2] : "UseDevelopmentStorage=true";
    const reset = process.argv[3] === "--reset" || process.argv[3] === "-r" ? true : false;

    const tableServiceClient = TableServiceClient.fromConnectionString(connectionString);

    async function getTables(tableServiceClient) {
        let tables = [];
        for await (const table of tableServiceClient.listTables()) {
            tables.push(table.name)
        }
        return tables;
    }

    if (reset) {
        const tables = await getTables(tableServiceClient);
        tables.forEach(async table => {
            const tableClient = TableClient.fromConnectionString(connectionString, table);
            console.log(`Deleting table: ${table}`);
            await tableClient.deleteTable();
        });
        let tablesExist = true;
        while (tablesExist) {
            console.log("Waiting for tables to be deleted...");
            const tables = await getTables(tableServiceClient);
            if (tables.length === 0) {
                tablesExist = false;
                console.log("All tables deleted.");
            }
            await new Promise(resolve => setTimeout(resolve, 1000));
        }
    }

    const jsonString = fs.readFileSync(path.resolve(__dirname, "products.json"), "utf8");
    const { products } = JSON.parse(jsonString);

    const tables = await getTables(tableServiceClient);

    if (tables.includes('products')) {
        console.log(`Table ${table} already exists, skipping...`);
        return;
    }

    let tableCreated = false;
    while (!tableCreated) {
        try {
            await tableServiceClient.createTable('products');
            tableCreated = true;
        } catch (err) {
            if (err.statusCode === 409) {
                console.log('Table is marked for deletion, retrying in 5 seconds...');
                await new Promise(resolve => setTimeout(resolve, 5000));
            } else {
                throw err;
            }
        }
    }

    const tableClient = TableClient.fromConnectionString(connectionString, 'products');

    const rows = products.map(product => {
        let {
            _id: rowKey,
            categories_hierarchy,
            ecoscore_grade,
            image_url,
            ingredients_text_en: ingredients_text,
            last_modified_t,
            nutriscore_grade,
            product_name,
            traces_tags,
            url
        } = product;

        traces_tags = traces_tags.join(', ');
        const categories = categories_hierarchy.join(', ');

        return {
            rowKey,
            categories,
            ecoscore_grade,
            image_url,
            ingredients_text,
            last_modified_t,
            nutriscore_grade,
            product_name,
            traces_tags,
            url
        }
    });

    rows.forEach(async row => {
        console.log(`Adding ${row.product_name}...`);
        await tableClient.createEntity({
            partitionKey: 'products',
            ...row
        });
    });

})();