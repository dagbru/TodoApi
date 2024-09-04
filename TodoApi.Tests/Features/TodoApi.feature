Feature: Todo Api
A basic todo api

    Scenario: Get all open items
        Given the following todo items are in the database
          | Id | Title | Text  | CreatedAt               | UpdatedAt | Completed | CompletedAt |
          | 1  | Test  | Test2 | 2024-08-27T07:44:56.837 |           | false     |             |
          | 2  | Test  | Test2 | 2024-08-27T07:44:56.837 |           | true      |             |
        When I call GET endpoint /todo/open
        Then the returned response status code is 200
        And the returned response is
        """
        [
            {
            "id": 1,
            "title": "Test",
            "text": "Test2",
            "createdAt": "2024-08-27T07:44:56.837",
            "updatedAt": null,
            "completed": false,
            "completedAt": null
            }
        ]
        """

    Scenario: Add a new todo item
        When I call POST endpoint /todo/add with payload
        """
        {
            "title": "d60653f3-c0a9-4063-8a24-81e7b48f4304",
            "text": "b037a730-2e2d-46cd-b59e-62f5626d79be"
        }
        """
        Then the returned response status code is 200
        And the following todo item exists in the database
          | Title                                | Text                                 | Completed |
          | d60653f3-c0a9-4063-8a24-81e7b48f4304 | b037a730-2e2d-46cd-b59e-62f5626d79be | false     |

    Scenario: Update existing todo item
        Given the following todo items are in the database
          | Id | Title | Text  | CreatedAt               | UpdatedAt | Completed | CompletedAt |
          | 1  | Test  | Test2 | 2024-08-27T07:44:56.837 |           | false     |             |
          When I call PUT endpoint /todo/update/1 with payload
          """
          {
            "title": "bbf88146-8aed-4fd3-8022-9b019ffd41d4",
            "text": "0b1dfa15-6dca-449d-acf7-a905d4be2549"
          }          
          """
        Then the returned response status code is 200
        And the following todo item exists in the database
          | Title                                | Text                                 | Completed |
          | bbf88146-8aed-4fd3-8022-9b019ffd41d4 | 0b1dfa15-6dca-449d-acf7-a905d4be2549 | false     |
        