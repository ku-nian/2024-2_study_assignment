using System;

namespace star
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the radius: ");
            int radius = int.Parse(Console.ReadLine());
            int size = 2 * (radius + 1);


            char[] star = new char[size*2];
            for (int i = 0; i < 2*size; i++) {
                star[i] = ' ';
            }


            bool flag = true;
            for (int i = 0; i < size; i++) {
                if (i == 2) {
                    star[1] = '*';
                    for (int j = 2; j < size - 1; j++) {
                        star[j] = ' ';
                    }
                }

                if (i == 1 || i == size-1) {
                    star[1] = ' '; 
                    for (int j = 2; j < size - 1; j++) {
                        star[j] = '*';
                    }
                }

                if (flag) {
                    for (int j = 0; j < size; j++) {
                        star[size + j] = ' ';
                    }
                    star[size + size/3] = '*'; // 4,6,8 / 3 -> 1,2,2 // 4,6,8 % 3 -> 1,0,2
                    star[size + size/3*2 + ((size%3 == 2) ? 1 : 0)] = '*';
                    flag = false;
                }

                if (i == size/3 || i == size/3*2 + ((size%3 == 2) ? 1 : 0)) {
                    for (int j = 0; j < size; j++) {
                        star[size + j] = '*';
                    }
                    flag = true;
                }
                Console.WriteLine(star);
                
            }
        }

        // calculate the distance between (x1, y1) and (x2, y2)
        static double SqrDistance2D(double x1, double y1, double x2, double y2)
        {
            return Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2);
        }

        // Checks if two values a and b are within a given distance.
        // |a - b| < distance
        static bool IsClose(double a, double b, double distance)
        {
            return Math.Abs(a - b) < distance;
        }
    }
}


/* example output
Enter the radius: 
>> 5
                *   *   
  *********     *   *   
 *              *   *   
 *              *   *   
 *          ************
 *              *   *   
 *              *   *   
 *              *   *   
 *          ************
 *              *   *   
 *              *   *   
  *********     *   *   

*/

/* example output (CHALLANGE)
Enter the radius: 
>> 5
                *   *  
      *         *   *  
   *     *      *   *  
  *                    
           ************
               *   *   
 *             *   *   
               *   *   
           ************
  *                    
   *     *    *   *    
      *       *   *    

*/
