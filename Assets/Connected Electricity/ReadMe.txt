Thank you for downloading !
Connected Electricity offers shader that simulate dynamic electricity between two node.

How to setup ?
=> Shader depend on a 3d noise texture, so all we need to do is set texture uniform on this material.
   1. The script "Noise3D.cs" is a 3d noise generator, call "Create" function to generate a 3d noise texture.
   2. Then just call "SetTexture" transfer the 3d noise to material applyed on the game object.
   3. Yes of course you can use your own 3d noise generator, it may produce something else interesting effects.
   
=> Please open "Demo.unity" to see the effect.
   1. The script "Demo.cs" is a demonstration about how to use effect in unity.
   2. In script "Demo.cs" we use an array to hold all "electricity objects" and transfer the generated 3d noise texture to their material when start.
   3. In demo scene we apply electricity material on quad and sphere mesh, in fact you are free to apply electricity material on any kind of mesh with uv coord between 0 ~ 1.

If you like it, please vote me 5 star on asset store. Thanks so much !
Any suggestion or improvement you want, please contact qq_d_y@163.com.