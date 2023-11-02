export const resultLayout = {
  "type": "AdaptiveCard",
  "version": "1.3",
  "body": [
    {
      "type": "ColumnSet",
      "columns": [
        {
          "type": "Column",
          "width": "auto",
          "items": [
            {
              "type": "Image",
              "url": "https://upload.wikimedia.org/wikipedia/commons/c/c7/OpenFoodFacts-logo.webp",
              "size": "Small",
              "horizontalAlignment": "Center",
              "altText": "Result logo"
            }
          ],
          "height": "stretch"
        },
        {
          "type": "Column",
          "width": 8,
          "items": [
            {
              "type": "TextBlock",
              "text": "[${name}](${url})",
              "color": "Accent",
              "size": "Medium",
              "weight": "Bolder"
            },
            {
              "type": "TextBlock",
              "text": "**Categories:** ${join(categories, ', ')}",
              "spacing": "Small",
              "$when": "${categories!=''}"
            },
            {
              "type": "TextBlock",
              "text": "**Ingredients:** ${join(ingredients, ', ')}",
              "spacing": "Small",
              "$when": "${ingredients!=''}"
            },
            {
              "type": "ColumnSet",
              "columns": [
                {
                  "type": "Column",
                  "width": "stretch",
                  "items": [
                    {
                      "type": "TextBlock",
                      "text": "**Nutriscore:** ${nutriscore}",
                      "spacing": "Small"
                    }
                  ]
                },
                {
                  "type": "Column",
                  "width": "stretch",
                  "items": [
                    {
                      "type": "TextBlock",
                      "text": "**Ecoscore:** ${ecoscore}",
                      "spacing": "Small"
                    }
                  ]
                }
              ]
            }
          ],
          "horizontalAlignment": "Center",
          "spacing": "Medium"
        },
        {
          "type": "Column",
          "width": 2,
          "items": [
            {
              "type": "Image",
              "url": "${imageUrl}",
              "altText": "Result logo",
              "size": "Medium",
              "horizontalAlignment": "Right"
            }
          ],
          "$when": "${imageUrl != ''}"
        }
      ]
    }
  ],
  "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
  "$data": {
    "ingredients": [
      "cocoa mass",
      "sugar"
    ],
    "categories": [
      "chocolate",
      "snacks",
      "sweet snacks"
    ],
    "nutriscore": "e",
    "url": "https://modernacdesigner.azurewebsites.net",
    "ecoscore": "d",
    "imageUrl": "https://searchuxcdn.azureedge.net/designerapp/images/stock-image.png",
    "name": "Contoso Marketing Analysis - Q3 FY18"
  }
};