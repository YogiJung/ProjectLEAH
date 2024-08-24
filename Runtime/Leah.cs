using System.Collections.Generic;
using System.Linq;
using ProjectLeah.Runtime.Objects;
using ProjectLeah.Runtime.Utils;
using UnityEngine;

namespace ProjectLeah.Runtime
{
   using System;
   using System.Threading.Tasks;
   using static MainType;
   
   public class Leah
   {
       private static Leah _instance;
       public static Leah Instance
       {
           get
           {
               if (_instance == null)
               {
                   _instance = new Leah();
               }
               return _instance;
           }
       }
       private ThreadPoolManager ThreadPoolManager { get; set; }
       private List<ManagedObject> _managedObjects;
       
       private Leah()
       {
           ThreadPoolManager = new ThreadPoolManager(2, 20);
           _managedObjects = new List<ManagedObject>();
       }

       private Leah(int min_n_of_thread, int max_n_of_thread)
       {
           ThreadPoolManager = new ThreadPoolManager(min_n_of_thread, max_n_of_thread);
           _managedObjects = new List<ManagedObject>();
       }
       
       private string HOST;
       private int PORT = 0;
       private ConnectionType connectionType;
       private int n_of_api = 0;
       private int n_of_thread = 10;
   
       public ManagedObject CreateObject(ConnectionType connectionType, string HOST, int PORT, int n_of_api ,int n_of_thread = 2) {
           string id = UUIDGenerator.GenerateRandomId();
           var managedObject = new ManagedObject
           {
               id = id,
               host = HOST,
               port = PORT,
               connectionType = connectionType,
               n_of_api = n_of_api,
               n_of_thread = n_of_thread
           };

           _managedObjects.Add(managedObject);

           ThreadPoolManager.ManageThreads(managedObject);

           return managedObject;
       }

       public void DeleteObject(string id)
       {
           var managedObject = _managedObjects.FirstOrDefault(obj => obj.id == id);
        
           if (managedObject != null)
           {
               _managedObjects.Remove(managedObject);
               
               managedObject = null;
            
               GC.Collect();
               GC.WaitForPendingFinalizers();
           }
           else
           {
               Console.WriteLine($"Managed object with ID {id} not found.");
           }
       }
   
       // public async Task<int> Connect(string id) {
       //     var managedObject = _managedObjects.FirstOrDefault(obj => obj.id == id);
       //
       //     if (managedObject != null)
       //     {
       //         try
       //         {
       //             managedObject.Connect();
       //         }
       //         catch (Exception e)
       //         {
       //             Debug.LogError($"ManagedObject Connection Error {e}");
       //             return -1;
       //         }
       //     }
       //     
       //     return 1;
       // }

       public List<ManagedObject> GetManagedObjects()
       {
           return _managedObjects;
       }
       // -1 : connection flag error, -2 : TCP Connection Failed, -3 : UDP Connection Failed,
       // 1 : connection Succeeded
   
   
       //networking => Separated ThreadPool with asynchronized communication with server
       //one thread with semaphore which is operated as shared memory space. ( Two Thread with Two Semaphore and Queue)
       // Main Thread with Unity will process sending data and receiving data through those threads.
   
       //Refactoring and Organization
       //Pipeline Generation
       
   }

}