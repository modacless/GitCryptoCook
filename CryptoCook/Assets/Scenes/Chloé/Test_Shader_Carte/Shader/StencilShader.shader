Shader "Examples/Stencil"
{
    SubShader
    {
        // The rest of the code that defines the SubShader goes here.

       Pass
       {
           ColorMask 0
           Zwrite Off

            Stencil
            {
                Ref 2
                Comp Always
                Pass Replace
            }

        // The rest of the code that defines the Pass goes here.
   }
    }
}