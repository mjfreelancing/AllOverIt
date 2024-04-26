﻿namespace NamedPipeTypes
{
    public sealed class PipeMessage
    {
        public class ChildClass
        {
            public int Value { get; set; } = (int) DateTime.Now.Ticks;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; }

        public ChildClass Child { get; set; } = new();
        public ChildClass NullChild { get; set; }

        public override string ToString()
        {
            return $"\"{Text}\" (message ID = {Id})";
        }
    }
}