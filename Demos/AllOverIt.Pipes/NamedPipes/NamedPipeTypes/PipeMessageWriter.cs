﻿using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;

namespace NamedPipeTypes
{
    public sealed class PipeMessageWriter : EnrichedBinaryValueWriter<PipeMessage>
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var message = (PipeMessage) value;

            writer.WriteGuid(message.Id);
            writer.WriteSafeString(message.Text);
            writer.WriteObject(message.Child, typeof(PipeMessage.ChildClass));
        }
    }
}