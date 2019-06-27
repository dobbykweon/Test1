using System;


public interface IDispatcher
{
    void Enqueue(Action action);
}
