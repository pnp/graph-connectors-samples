#!/bin/bash

mgc login --scopes AppRoleAssignment.ReadWrite.All Application.ReadWrite.All --strategy InteractiveBrowser

# create the application
app=$(mgc applications create --body '{"displayName":"Waldek Mastykarz (blog) - connector (Python)","requiredResourceAccess":[{"resourceAccess":[{"id":"f431331c-49a6-499f-be1c-62af19c34a9d","type":"Role"},{"id":"8116ae0f-55c2-452d-9944-d18420f5b2c8","type":"Role"}],"resourceAppId":"00000003-0000-0000-c000-000000000000"}]}')
appId=$(echo $app | jq -r '.appId')
appObjectId=$(echo $app | jq -r '.id')

# grant admin consent
graphSpId=$(mgc service-principals list --filter "appId eq '00000003-0000-0000-c000-000000000000'" --select id --query 'value[0].id')
# remove surrounding quotes
graphSpId="${graphSpId#\"}"
graphSpId="${graphSpId%\"}"

sp=$(mgc service-principals create --body '{"appId":"'"${appId}"'"}')
spId=$(echo $sp | jq -r '.id')
mgc service-principals app-role-assignments create --service-principal-id $spId --body '{ "principalId": "'"${spId}"'", "resourceId": "'"${graphSpId}"'", "appRoleId": "f431331c-49a6-499f-be1c-62af19c34a9d" }' --output none
mgc service-principals app-role-assignments create --service-principal-id $spId --body '{ "principalId": "'"${spId}"'", "resourceId": "'"${graphSpId}"'", "appRoleId": "8116ae0f-55c2-452d-9944-d18420f5b2c8" }' --output none

# create client secret
cred=$(mgc applications add-password post --application-id $appObjectId --body '{}')
secret=$(echo $cred | jq -r '.secretText')

tenantId=$(mgc organization list --query 'value[0].id')
# remove surrounding quotes
tenantId="${tenantId#\"}"
tenantId="${tenantId%\"}"

# store values
cat << EOF > config.ini
[AZURE]
CLIENT_ID = $appId
CLIENT_SECRET = $secret
TENANT_ID = $tenantId
EOF
