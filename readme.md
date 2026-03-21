## Usage example log

info: EventBasedCommunicationService.Implementation.EventService[0]
      Binding "event-based-communication" exchange with queue: "amq.gen-BSWkjltv2RPKZ3jJYoZ-xg" by binding key: "message.event"

info: EventBasedCommunicationService.Implementation.EventService[0]
      Binding "event-based-communication" exchange with queue: "amq.gen-BSWkjltv2RPKZ3jJYoZ-xg" by binding key: "user.updated"

info: EventBasedCommunicationService.Implementation.EventService[0]
      Waiting for messages...

info: EventBasedCommunicationService.Consumer.Models.Events.UserUpdatedHandler[0]
      "UserUpdatedHandler" handle "UserUpdated" event "17b9b9d6-fad0-4947-9017-d6bbd4d08917"

info: EventBasedCommunicationService.Consumer.Models.Events.UserUpdatedHandler[0]
      User { Id = 8d90aaf4-8fd5-42e7-b668-daa9c18d7a66, Username = John Doe } has been updated

info: EventBasedCommunicationService.Consumer.Models.Events.EchoMessageEventHandler[0]
      Received message "231e13f2-3351-4cc5-88e1-b872fa9a641d" (IsLast = False) from user "111e13f2-3351-4cc5-88e1-b872fa9a641d": "hello!"

info: EventBasedCommunicationService.Consumer.Models.Events.EchoMessageEventHandler[0]
      Echo message "111e13f2-3351-4cc5-88e1-b872fa9a641d" to "event-based-communication" exchange.

info: EventBasedCommunicationService.Implementation.EventService[0]
      Sent message with "message.event" routing key to "event-based-communication" exchange: {"IsLast":true,"UserId":"231e13f2-3351-4cc5-88e1-b872fa9a641d","Message":"hello!","RoutingKey":"message.event","Id":"111e13f2-3351-4cc5-88e1-b872fa9a641d","CreatedAt":"2026-03-21T19:06:52.2022409+00:00"}
      
info: EventBasedCommunicationService.Consumer.Models.Events.EchoMessageEventHandler[0]
      Received message "231e13f2-3351-4cc5-88e1-b872fa9a641d" (IsLast = True) from user "111e13f2-3351-4cc5-88e1-b872fa9a641d": "hello!"
info: EventBasedCommunicationService.Consumer.Models.Events.EchoMessageEventHandler[0]
      It was last step for message "111e13f2-3351-4cc5-88e1-b872fa9a641d"
