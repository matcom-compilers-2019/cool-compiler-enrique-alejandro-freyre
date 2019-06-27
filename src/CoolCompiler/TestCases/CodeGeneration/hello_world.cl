class Main inherits IO {
   main(): IO {
	(let x : Int in
	 	{
            	x <- 5;
		out_string(x.copy() + 5);
	 }
      )
	
   };};