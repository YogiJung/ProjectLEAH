using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProjectLeah.Runtime.Objects;
using UnityEditor.PackageManager;
using UnityEngine;
using static MainType;

internal class ThreadPoolManager
{
    internal ThreadPoolManager(int minSize, int maxSize)
    {
        ThreadPool.SetMaxThreads(maxSize, maxSize);
        ThreadPool.SetMinThreads(minSize, minSize);
    }
    
    public void ManageThreads(ManagedObject managedObject)
    {
        for (int i = 0; i < managedObject.n_of_thread; i++)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"Managing {managedObject.id} on thread {i+1}/{managedObject.n_of_thread}");
            });
        }
    }
}