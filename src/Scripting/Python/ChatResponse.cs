namespace Stonebot.Scripting.Python {
    using Models.Responses;
    using System.Linq;

    public class ChatResponse(PostChatMessageResponse response) {
        public readonly DataPoint[] data = [.. response.Data.Select(dataPoint => new DataPoint(dataPoint))];
        public readonly DropReason drop_reason = new(response.DropReason);

        public class DataPoint(SendChatMessageResponseDataPoint dataPoint) {
            public readonly string message_id = dataPoint.MessageId;
            public readonly bool is_sent = dataPoint.IsSent;
        }

        public class DropReason(SendChatMessageResponseDropReason dropReason) {
            public readonly string code = dropReason.Code;
            public readonly string message = dropReason.Message;
        }
    }
}
