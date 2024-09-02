Feature: Todo
A simple todo API

    Scenario: List open items
        Given the following todo items in database
          | Id | Title | Text     | CreatedAt  | UpdatedAt  | Completed | CompletedAt |
          | 1  | Test  | Testtext | 2024-02-09 | 2024-02-09 | false     |             |
          | 2  | Test  | Testtext | 2024-02-09 | 2024-02-09 | true      |             |
        When I run a GET request with endpoint /open
        Then the returned status should be code 200
        And the result should be
        """
        [
            {
                "id": 1,
                "title": "Test",
                "text": "Testtext",
                "createdAt": "2024-02-09",
                "updatedAt": "2024-02-09",
                "completed": false,
                "completedAt": null
            }
        ]
        """

    Scenario: Add new todo item
        When I run a POST request with endpoint /add
        """
            {
            "title": "93c86eab-a71b-4d0a-ae01-101c4ea2e945",
            "text": "70f7ba6c-a066-48e0-bb98-cdfce6ec7030"
            }
        """
        Then the returned status should be code 200
        And the following todo item should exist in the database
          | Title                                | Text                                 | Completed |
          | 93c86eab-a71b-4d0a-ae01-101c4ea2e945 | 70f7ba6c-a066-48e0-bb98-cdfce6ec7030 | false     |

    Scenario: Update title and text
        Given the following todo items in database
          | Id | Title        | Text       | CreatedAt  | UpdatedAt  | Completed | CompletedAt |
          | 1  | Inital title | First text | 2024-02-09 | 2024-02-09 | false     |             |
        When I run PUT request with endpoint /update/1
        """
        {
            "title": "910ef365-b4a7-4b60-a123-85019d86cd8c",
            "text": "78d52a51-78c3-4e98-9b39-dfa8d805434d"
        }
        """
        Then the returned status should be code 200
        And the following todo item should exist in the database
          | Title                                | Text                                 | Completed |
          | 910ef365-b4a7-4b60-a123-85019d86cd8c | 78d52a51-78c3-4e98-9b39-dfa8d805434d | false     |