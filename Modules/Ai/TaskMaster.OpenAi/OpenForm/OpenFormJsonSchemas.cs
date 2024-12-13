namespace TaskMaster.OpenAi.OpenForm;

internal static class OpenFormJsonSchemas
{
    internal const string MailJsonSchema = """
                                          {
                                            "$schema": "http://json-schema.org/draft-07/schema#",
                                            "title": "Mail",
                                            "type": "object",
                                            "properties": {
                                              "Header": {
                                                "type": "object",
                                                "properties": {
                                                  "Title": {
                                                    "type": "string",
                                                    "description": "The title of the exercise."
                                                  },
                                                  "TaskDescription": {
                                                    "type": "string",
                                                    "description": "The task description for the exercise."
                                                  },
                                                  "Instruction": {
                                                    "type": "string",
                                                    "description": "The instructions for the exercise."
                                                  },
                                                  "Example": {
                                                    "type": "string",
                                                    "description": "An example related to the exercise."
                                                  },
                                                  "SupportMaterial": {
                                                    "type": "string",
                                                    "description": "Support material for the exercise, if any."
                                                  }
                                                },
                                                "additionalProperties": false,
                                                "required": ["Title", "TaskDescription", "Instruction", "Example", "SupportMaterial"]
                                              }
                                            },
                                            "additionalProperties": false,
                                            "required": ["Header"]
                                          }
                                          """;

    internal const string EssayJsonSchema = """
                                           {
                                             "$schema": "http://json-schema.org/draft-07/schema#",
                                             "title": "Essay",
                                             "type": "object",
                                             "properties": {
                                               "Header": {
                                                 "type": "object",
                                                 "properties": {
                                                   "Title": {
                                                     "type": "string",
                                                     "description": "The title of the exercise."
                                                   },
                                                   "TaskDescription": {
                                                     "type": "string",
                                                     "description": "The task description for the exercise."
                                                   },
                                                   "Instruction": {
                                                     "type": "string",
                                                     "description": "The instructions for the exercise."
                                                   },
                                                   "Example": {
                                                     "type": "string",
                                                     "description": "An example related to the exercise."
                                                   },
                                                   "SupportMaterial": {
                                                     "type": "string",
                                                     "description": "Support material for the exercise, if any."
                                                   }
                                                 },
                                                 "additionalProperties": false,
                                                 "required": ["Title", "TaskDescription", "Instruction", "Example", "SupportMaterial"]
                                               }
                                             },
                                             "additionalProperties": false,
                                             "required": ["Header"]
                                           }
                                           """;

    internal const string SummaryOfTextJsonSchema = """
                                                   {
                                                     "$schema": "http://json-schema.org/draft-07/schema#",
                                                     "title": "SummaryOfText",
                                                     "type": "object",
                                                     "properties": {
                                                       "Header": {
                                                         "type": "object",
                                                         "properties": {
                                                           "Title": {
                                                             "type": "string",
                                                             "description": "The title of the exercise."
                                                           },
                                                           "TaskDescription": {
                                                             "type": "string",
                                                             "description": "The task description for the exercise."
                                                           },
                                                           "Instruction": {
                                                             "type": "string",
                                                             "description": "The instructions for the exercise."
                                                           },
                                                           "Example": {
                                                             "type": "string",
                                                             "description": "An example related to the exercise."
                                                           },
                                                           "SupportMaterial": {
                                                             "type": "string",
                                                             "description": "Support material for the exercise, if any."
                                                           }
                                                         },
                                                         "additionalProperties": false,
                                                         "required": ["Title", "TaskDescription", "Instruction", "Example", "SupportMaterial"]
                                                       },
                                                       "TextToSummary": {
                                                         "type": "string",
                                                         "description": "The text that needs to be summarized."
                                                       }
                                                     },
                                                     "additionalProperties": false,
                                                     "required": ["Header", "TextToSummary"]
                                                   }
                                                   """;
}