// using UnityEngine;
//
// namespace Drongo.GameboyEmulator
// {
//     public class PPU_TexturePaint : PPU
//     {
//         private Texture2D _texture;
//         
//         public PPU_TexturePaint(AddressBus bus) : base(bus)
//         {
//             _texture = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT, TextureFormat.RGB24, false);
//             _texture.filterMode = FilterMode.Point;
//         }
//
//         private void SetPixel()
//         {
//             int x, y;
//             for (int i = 0; i < pixels.Length; ++i)
//             {
//                 x = i % SCREEN_WIDTH;
//                 y = i / SCREEN_WIDTH;
//                 _texture.SetPixel(x, SCREEN_HEIGHT - 1 - y, NumberToColor(pixels [i]));
//             }
//             _texture.Apply();
//         }
//         
//         private Color NumberToColor(uint color)
//         {
//             byte r = (byte)(color >> 16);
//             byte g = (byte)(color >> 8);
//             byte b = (byte)(color >> 0);
//             return new Color(r / 255f, g / 255f, b / 255f, 255f);
//         }
//         
//     }
// }