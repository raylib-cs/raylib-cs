#version 100

// value reaches scale*scale = 1,000,000, beyond mediump's guaranteed precision
#ifdef GL_FRAGMENT_PRECISION_HIGH
precision highp float;
#else
precision mediump float;
#endif

/*************************************************************************************

  The Sieve of Eratosthenes -- a simple shader by ProfJski
  An early prime number sieve: https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes

  The screen is divided into a square grid of boxes, each representing an integer value
  Each integer is tested to see if it is a prime number.  Primes are colored white
  Non-primes are colored with a color that indicates the smallest factor which evenly divides our integer

  You can change the scale variable to make a larger or smaller grid
  Total number of integers displayed = scale squared, so scale = 100 tests the first 10,000 integers

  WARNING: If you make scale too large, your GPU may bog down!

  NOTE: GLSL ES 100 requires constant loop bounds, so the factor loop runs to the worst
  case (scale) and breaks out at sqrt(value), matching the glsl330 version

***************************************************************************************/

// Input vertex attributes (from vertex shader)
varying vec2 fragTexCoord;
varying vec4 fragColor;

// Make a nice spectrum of colors based on counter and maxSize
vec4 Colorizer(float counter, float maxSize)
{
    float red = 0.0, green = 0.0, blue = 0.0;
    float normsize = counter/maxSize;

    red = smoothstep(0.3, 0.7, normsize);
    green = sin(3.14159*normsize);
    blue = 1.0 - smoothstep(0.0, 0.4, normsize);

    return vec4(0.8*red, 0.8*green, 0.8*blue, 1.0);
}

void main()
{
    vec4 color = vec4(1.0);
    float scale = 1000.0; // Makes 100x100 square grid, change this variable to make a smaller or larger grid
    float value = scale*floor(fragTexCoord.y*scale) + floor(fragTexCoord.x*scale);  // Group pixels into boxes representing integer values

    if (value <= 2.0) gl_FragColor = vec4(1.0);
    else
    {
        float maxFactor = max(2.0, sqrt(value) + 1.0);

        for (int i = 2; i < 1001; i++)  // Constant bound = scale (worst-case sqrt(value))
        {
            if (float(i) >= maxFactor) break;

            if ((value - float(i)*floor(value/float(i))) <= 0.0)
            {
                color = Colorizer(float(i), scale);
                //break;    // Uncomment to color by the largest factor instead
            }
        }

        gl_FragColor = color;
    }
}
