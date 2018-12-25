namespace Engine

type IHost =
   //// abstract method
   //abstract member Add: int -> int -> int

   //// abstract immutable property
   //abstract member Pi : float 

   //// abstract read/write property
   abstract member RecentPath : string with get, set

