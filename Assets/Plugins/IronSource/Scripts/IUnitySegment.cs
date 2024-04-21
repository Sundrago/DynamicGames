using System;

public interface IUnitySegment
{
    event Action<string> OnSegmentRecieved;
}