# StockTrader
An experimental Blazor WASM app to see if "real work" can be accomplished using Web Assembly.  The purpose of the app is to see if it is possible for long running processes that would normally be run as threads on a traditional application can be distributed over the internet using Web Assembly.

I am converting a traditional console app (my stock trader app) that runs permutations of stock trading approaches to find trading patterns within blue chip stock flucuations.  The full app will not be converted, but rather just enough to determine if "real work" can be distributed using WASM.

# Status:  Unfinished

The app is currently unfinished, however many approaches have been tested and several findings have already been uncovered.  The UI has not been fleshed out at all aside from being able to get to the core functionality.

# Current Findings

All Web Assembly apps currently run in a **single thread**.  
* Yielding execution to the UI (in this case, Blazor WASM), causes significant slow downs and doesn't allow work to really be done in the *Background*
* Not yielding, causes the UI to hang as in other single threaded applications.
* Both Chrome and Edge will prompt to close the application if the application is in a tight loop and cannot respond.  This forces apps to yield no matter what.
* It is possible to open a new tab from the application which will give another "thread" of execution.  This is the current approach I am using to perform work, however, the application still must have a UI and still must yield execution so that the browswer does not close the application.

# Research & Future
* **As of November 2020**, the Web Assembly standard **does** support multi-threading. 
* Both Chrome and Edge support Web Assembly multi-threading.
* The current versions of Mono (on which Blazor and Uno Platform are built) support multi-threading
* **However**, Blazor Web Assembly is built upon an older Mono release, and does not support multi-threading.  In addition, it does not appear that multi-threading support is on the roadmap for .NET 6 which is to be released in Novermber 2021.  Thus true multi-threading support may not be available in Blazor WASM for quite some time.


## Uno Platform on Web Assembly with Multi-Threading
The [Uno Platform](https://platform.uno/blog/webassembly-threading-in-net/) does support multi-threading on Web Assembly.  Unlike Blazor, it is built on a newer version of Mono.

**However**, after experimentation, the suuport is not currently fully fleshed out.  Two threads do run as expected, however if you attempt to spin up more threads, they do not start and no errors or warnings are given in the browser console.  The initial assumption is 2 threads are supported and the other threads will run from the threadpool once one thread finishes, but this is not the case. The threads that are queued in the threadpool never execute and are essentially lost.

In addition, the Uno Platorm is geared towards building apps that run everywhere.  This means the nice things to have (like url parsing and multiple endpoints) do not appear to be available using Uno.  This makes it less desirable (for me at least) for distributing work using Uno.

For these reasons, I have decided not to proceed with Uno Platform.


