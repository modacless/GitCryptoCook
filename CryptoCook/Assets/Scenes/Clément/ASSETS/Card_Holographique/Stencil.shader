Shader "Custom/Stencil"
{
    SubShader
    {
        pass
        {

            ColorMask 0
            Zwrite Off

         Stencil
         {
            Ref 2
            Comp Always
            Pass Replace
          }





        }

    }
}  
