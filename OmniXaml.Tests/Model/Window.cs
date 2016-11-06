﻿namespace OmniXaml.Tests.Model
{
    using DefaultLoader;

    [Namescope]
    internal class Window : ModelObject
    {
        public static readonly object LoadedEvent = new object();

        public string Title { get; set; }
        [Content]
        public object Content { get; set; }

        public override string ToString()
        {
            return $"{nameof(Title)}: {Title}, {nameof(Content)}: {Content}";
        }

        protected bool Equals(Window other)
        {
            return string.Equals(Title, other.Title) && Equals(Content, other.Content);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Window) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Title != null ? Title.GetHashCode() : 0)*397) ^ (Content != null ? Content.GetHashCode() : 0);
            }
        }
    }
}