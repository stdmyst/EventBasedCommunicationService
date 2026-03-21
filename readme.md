## Usage example log

info: EventBasedCommunicationService.Implementation.EventService[0]
      Binding "event-based-communication" exchange with queue: "amq.gen-1EypyG55hqHjVdvw04lqdw" by binding key: "message.event"

info: EventBasedCommunicationService.Implementation.EventService[0]
      Binding "event-based-communication" exchange with queue: "amq.gen-1EypyG55hqHjVdvw04lqdw" by binding key: "user.updated"

info: EventBasedCommunicationService.Implementation.EventService[0]
      Waiting for messages...

info: EventBasedCommunicationService.Consumer.Models.Events.MessageEvent[0]
      Received message "231e13f2-3351-4cc5-88e1-b872fa9a641d" (IsLast = False) from user "111e13f2-3351-4cc5-88e1-b872fa9a641d": "hello!"

info: EventBasedCommunicationService.Consumer.Models.Events.MessageEvent[0]
      Echo message "111e13f2-3351-4cc5-88e1-b872fa9a641d" to "event-based-communication" exchange.

info: EventBasedCommunicationService.Implementation.EventService[0]
      Sent message with "message.event" routing key to "event-based-communication" exchange: {"IsLast":true,"UserId":"231e13f2-3351-4cc5-88e1-b872fa9a641d","Message":"hello!","RoutingKey":"message.event","Id":"111e13f2-3351-4cc5-88e1-b872fa9a641d","CreatedAt":"2026-03-21T18:52:12.9680821+00:00"}

info: EventBasedCommunicationService.Consumer.Models.Events.MessageEvent[0]
      Received message "231e13f2-3351-4cc5-88e1-b872fa9a641d" (IsLast = True) from user "111e13f2-3351-4cc5-88e1-b872fa9a641d": "hello!"

info: EventBasedCommunicationService.Consumer.Models.Events.MessageEvent[0]
      It was last step for message "111e13f2-3351-4cc5-88e1-b872fa9a641d"

info: EventBasedCommunicationService.Consumer.Models.Events.UserUpdatedHandler[0]
      "UserUpdatedHandler" handle "UserUpdated" event "c188fae4-27f4-4c71-bea8-4e2f7f9f6d34"

info: EventBasedCommunicationService.Consumer.Models.Events.UserUpdatedHandler[0]
      User { Id = e95e963b-ea89-4ca6-8464-e26c05398951, Username = John Doe } has been updated
