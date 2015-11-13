using UnityEngine;
using System.Collections;

public class Noise
{
	const int hashMask = 256;
	int[] hash = new int[hashMask + hashMask];

	float FADE(float t) { return t * t * t * ( t * ( t * 6.0f - 15.0f ) + 10.0f ); }
	
	float LERP(float t, float a, float b) { return (a) + (t)*((b)-(a)); }
	
	float GRAD1(int hash, float x ) 
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//		int h = hash % 16;
//		float grad = 1.0f + (h % 8);
//		if((h%8) < 4) grad = -grad;
//		return ( grad * x );
		
		int h = hash & 15;
    	float grad = 1.0f + (h & 7);
    	if ((h&8) != 0) grad = -grad;
    	return ( grad * x );
	}
	
	float perlinGrad(int hash, float x, float y)
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//		int h = hash % 16;
//    	float u = h<4 ? x : y;
//    	float v = h<4 ? y : x;
//		int hn = h%2;
//		int hm = (h/2)%2;
//    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -2.0f*v : 2.0f*v);
		
    	int h = hash & 7;
    	float u = h<4 ? x : y;
    	float v = h<4 ? y : x;
    	return (((h&1) != 0)? -u : u) + (((h&2) != 0) ? -2.0f*v : 2.0f*v);
	}
	

	float GRAD3(int hash, float x, float y , float z)
	{
		//This method uses the mod operator which is slower 
		//than bitwise operations but is included out of interest
//    	int h = hash % 16;
//    	float u = (h<8) ? x : y;
//    	float v = (h<4) ? y : (h==12||h==14) ? x : z;
//		int hn = h%2;
//		int hm = (h/2)%2;
//    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -v : v);
		
		int h = hash & 15;
    	float u = h<8 ? x : y;
    	float v = (h<4) ? y : (h==12 || h==14) ? x : z;
    	return (((h&1) != 0)? -u : u) + (((h&2) != 0)? -v : v);
	}
	
//	float Noise1D( float x )
//	{
//		//returns a noise value between -0.5 and 0.5
//	    int ix0, ix1;
//	    float fx0, fx1;
//	    float s, n0, n1;
//	
//	    ix0 = (int)Mathf.Floor(x); 	// Integer part of x
//	    fx0 = x - ix0;       	// Fractional part of x
//	    fx1 = fx0 - 1.0f;
//	    ix1 = ( ix0+1 ) & 0xff;
//	    ix0 = ix0 & 0xff;    	// Wrap to 0..255
//		
//	    s = FADE(fx0);
//	
//	    n0 = GRAD1(m_perm[ix0], fx0);
//	    n1 = GRAD1(m_perm[ix1], fx1);
//	    return 0.188f * LERP( s, n0, n1);
//	}
//	
	float perlinNoise( float x, float y )
	{
		//returns a noise value between -0.75 and 0.75
	    int ix0, iy0, ix1, iy1;
	    float fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;
	
	    ix0 = (int)Mathf.Floor(x); 	// Integer part of x
	    iy0 = (int)Mathf.Floor(y); 	// Integer part of y
	    fx0 = x - ix0;        	// Fractional part of x
	    fy0 = y - iy0;        	// Fractional part of y
	    fx1 = fx0 - 1.0f;
	    fy1 = fy0 - 1.0f;
	    ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
	    iy1 = (iy0 + 1) & 0xff;
	    ix0 = ix0 & 0xff;
	    iy0 = iy0 & 0xff;
	    
	    t = FADE( fy0 );
	    s = FADE( fx0 );
	
		nx0 = perlinGrad(hash[ix0 + hash[iy0]], fx0, fy0);
	    nx1 = perlinGrad(hash[ix0 + hash[iy1]], fx0, fy1);
		
	    n0 = LERP( t, nx0, nx1 );
	
	    nx0 = perlinGrad(hash[ix1 + hash[iy0]], fx1, fy0);
	    nx1 = perlinGrad(hash[ix1 + hash[iy1]], fx1, fy1);
		
	    n1 = LERP(t, nx0, nx1);
	
	    return 0.507f * LERP( s, n0, n1 );
	}

//	Vector2[] gradients2D = {
//		new Vector2(1f, 0f),
//		new Vector2(-1f, 0f),
//		new Vector2(0f, 1f),
//		new Vector2(0f, -1f),
//		new Vector2(-1f, 1f).normalized,
//		new Vector2(-1f, -1f).normalized,
//		new Vector2(1f, 1f).normalized,
//		new Vector2(1f, -1f).normalized
//
//	};
//	
//	float Dot(Vector2 grad, float x, float y){
//		return((grad.x * x) + (grad.y * y));
//	}
//	private const int gradientMask2D = 3;
//	
//	float perlinGrad(int hash, float x, float y){
//		Vector2 grad = gradients2D[hash & gradientMask2D];
//		return Dot (grad, x, y);
//	}

//	float ValueNoise2D( float x, float y )
//	{
//		//returns a noise value between -0.75 and 0.75
//		int ix0, iy0, ix1, iy1;
//		float fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;
//		
//		ix0 = (int)Mathf.Floor(x); 	// Integer part of x
//		iy0 = (int)Mathf.Floor(y); 	// Integer part of y
//		fx0 = x - ix0;        	// Fractional part of x
//		fy0 = y - iy0;        	// Fractional part of y
//		fx1 = fx0 - 1.0f;
//		fy1 = fy0 - 1.0f;
//		ix1 = (ix0 + 1) & 0xff; // Wrap to 0..255
//		iy1 = (iy0 + 1) & 0xff;
//		ix0 = ix0 & 0xff;
//		iy0 = iy0 & 0xff;
//		
//		t = FADE( fy0 );
//		s = FADE( fx0 );
//
//		int hx0 = hash [ix0];
//		int hx1 = hash [hx0 + ix1];
//		
//		int hy0 = hash [iy0];
//		int hy1 = hash [hy0 + iy1];
//
//		float h00 = perlinGrad (hash[ix0 + hash [ix0]], fx0, fy0);
//		float h01 = perlinGrad (hash [ix0 + hash [iy1]], fx0, fy1);
//		float h10 = perlinGrad (hash [ix1 + hash[iy0]], fx1, fx0);
//		float h11 = perlinGrad (hash [ix1 + hash[iy1]], fx1, fx0);
//
////		return (Mathf.Lerp (Mathf.Lerp (h00, h01, s), Mathf.Lerp (h01, h11, s), t));
//		return (LERP(t, LERP(s, h00, h01), LERP(s, h01, h11))) * Mathf.Sqrt(2f);
////		return 0.507f * LERP( s, h0, h1 );
//	}

//	float Noise3D( float x, float y, float z )
//	{
//		//returns a noise value between -1.5 and 1.5
//	    int ix0, iy0, ix1, iy1, iz0, iz1;
//	    float fx0, fy0, fz0, fx1, fy1, fz1;
//	    float s, t, r;
//	    float nxy0, nxy1, nx0, nx1, n0, n1;
//	
//	    ix0 = (int)Mathf.Floor( x ); // Integer part of x
//	    iy0 = (int)Mathf.Floor( y ); // Integer part of y
//	    iz0 = (int)Mathf.Floor( z ); // Integer part of z
//	    fx0 = x - ix0;        // Fractional part of x
//	    fy0 = y - iy0;        // Fractional part of y
//	    fz0 = z - iz0;        // Fractional part of z
//	    fx1 = fx0 - 1.0f;
//	    fy1 = fy0 - 1.0f;
//	    fz1 = fz0 - 1.0f;
//	    ix1 = ( ix0 + 1 ) & 0xff; // Wrap to 0..255
//	    iy1 = ( iy0 + 1 ) & 0xff;
//	    iz1 = ( iz0 + 1 ) & 0xff;
//	    ix0 = ix0 & 0xff;
//	    iy0 = iy0 & 0xff;
//	    iz0 = iz0 & 0xff;
//	    
//	    r = FADE( fz0 );
//	    t = FADE( fy0 );
//	    s = FADE( fx0 );
//	
//		nxy0 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz0]]], fx0, fy0, fz0);
//	    nxy1 = GRAD3(m_perm[ix0 + m_perm[iy0 + m_perm[iz1]]], fx0, fy0, fz1);
//	    nx0 = LERP( r, nxy0, nxy1 );
//	
//	    nxy0 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz0]]], fx0, fy1, fz0);
//	    nxy1 = GRAD3(m_perm[ix0 + m_perm[iy1 + m_perm[iz1]]], fx0, fy1, fz1);
//	    nx1 = LERP( r, nxy0, nxy1 );
//	
//	    n0 = LERP( t, nx0, nx1 );
//	
//	    nxy0 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz0]]], fx1, fy0, fz0);
//	    nxy1 = GRAD3(m_perm[ix1 + m_perm[iy0 + m_perm[iz1]]], fx1, fy0, fz1);
//	    nx0 = LERP( r, nxy0, nxy1 );
//	
//	    nxy0 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz0]]], fx1, fy1, fz0);
//	   	nxy1 = GRAD3(m_perm[ix1 + m_perm[iy1 + m_perm[iz1]]], fx1, fy1, fz1);
//	    nx1 = LERP( r, nxy0, nxy1 );
//	
//	    n1 = LERP( t, nx0, nx1 );
//	    
//	    return 0.936f * LERP( s, n0, n1 );
//	}
	
//	public float FractalNoise1D(float x, int octNum, float frq, float amp)
//	{
//		float gain = 1.0f;
//		float sum = 0.0f;
//	
//		for(int i = 0; i < octNum; i++)
//		{
//			sum +=  Noise1D(x*gain/frq) * amp/gain;
//			gain *= 2.0f;
//		}
//		return sum;
//	}
//	
	public float FractalNoise(float x, float y, int octNum, float frq, float amp)
	{
		float gain = 1.0f;
		float sum = 0.0f;
		
		for(int i = 0; i < octNum; i++)
		{
			sum += perlinNoise(x*gain/frq, y*gain/frq) * amp/gain;
			gain *= 2.0f;
		}
		return sum;
	}
	
//	public float FractalNoise3D(float x, float y, float z, int octNum, float frq, float amp)
//	{
//		float gain = 1.0f;
//		float sum = 0.0f;
//	
//		for(int i = 0; i < octNum; i++)
//		{
//			sum +=  Noise3D(x*gain/frq, y*gain/frq, z*gain/frq) * amp/gain;
//			gain *= 2.0f;
//		}
//		return sum;
//	}

	public void setPermutations(int seed)
	{
		Random.seed = seed;
		
		int i, j, k;
		//Assigining numbers sequentially
		for (i = 0 ; i < hashMask ; i++) 
		{
			hash[i] = i;
		}

		//Changes order of numbers based on random value
		while (--i != 0) 
		{
			k = hash[i];
			j = Random.Range(0, hashMask);
			hash[i] = hash[j];
			hash[j] = k;
		}
		// Duplicates current randomly ordered number & concatenates them.
		for (i = 0 ; i < hashMask; i++) 
		{
			hash[hashMask + i] = hash[i];
		}

	}

}













