using System.Collections;
using UnityEngine;

namespace Drongo.GameboyEmulator.Renderers
{
    public class Renderer_Gizmos : MonoBehaviour
    {
        [SerializeField]
        private Color[] _palette;
        
        private GameBoy _gb;
        private PPU _ppu;
        
        private IEnumerator Start()
        {
            yield return null;
            
            _gb = GetComponent<GameBoy>();
            _ppu = _gb.ppu;
        }

        private void OnDrawGizmos()
        {
            if(! Application.isPlaying)
                return;

            Vector3 pos = new Vector3();
            Vector3 size = Vector3.one * 0.1f;

            Color prevColor = Gizmos.color;
            
            for (int x = 0; x < PPU.SCREEN_WIDTH; x++)
            {
                for (int y = 0; y < PPU.SCREEN_HEIGHT; y++)
                {
                    pos.x = PPU.SCREEN_WIDTH * 0.1f + x * 0.1f;
                    pos.y = PPU.SCREEN_HEIGHT * 0.1f - y * 0.1f;

                    Gizmos.color = _palette[_ppu.pixels[x, y]];
                    Gizmos.DrawCube(pos, size);
                }
            }
            
            Gizmos.color = prevColor;
        }
    }
}