using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestSpriteEqual : SerializedMonoBehaviour
{
    public Sprite sprite;
    public List<Sprite> sprites;
    public Texture2D texture;

    [ReadOnly] [SerializeField] private Dictionary<Int64, List<Sprite>> map = new();

    [Button]
    private void Fill()
    {
        map.Clear();

        foreach (var spriteIt in sprites)
        {
            long spriteKey = GetSpriteKey(spriteIt);

            if (!map.TryGetValue(spriteKey, out List<Sprite> value))
            {
                value = new List<Sprite>();
                map[spriteKey] = value;
            }

            value.Add(spriteIt);
        }
    }

    private static long GetKeyFromTextureRawData(byte[] data)
    {
        long res = 0;

        const int PARTS = 20;
        int maxIndex = data.Length - sizeof(long);
        int step = maxIndex / PARTS;

        for (int i = 0; i < PARTS; i++)
        {
            res += BitConverter.ToInt64(data, i * step);
        }

        return res;
    }

    private static long GetTextureKey(Texture2D texture)
    {
        var data = texture.GetRawTextureData();

        return GetKeyFromTextureRawData(data);
    }

    private static long GetSpriteKey(Sprite spriteIt)
    {
        var data = spriteIt.texture.GetRawTextureData();

        return GetKeyFromTextureRawData(data);
    }

    [Button]
    private void Check()
    {
        List<Sprite> equalSprites = new List<Sprite>();

        foreach (var spriteIt in sprites)
        {
            if (IsEqualTextures(sprite, spriteIt))
            {
                equalSprites.Add(spriteIt);
            }
        }

        if (equalSprites.Count == 0)
        {
            Debug.Log("No one equal sprite found");
            return;
        }

        StringBuilder equalSpritesBuilder = new StringBuilder("Equal sprites:\n");

        foreach (var equalSpriteIt in equalSprites)
        {
            equalSpritesBuilder.Append(
                equalSpriteIt.name + $" Hash: {equalSpriteIt.texture.GetRawTextureData()}" + '\n');
        }

        Debug.Log(equalSpritesBuilder.ToString());
    }

    [Button]
    private void Check2()
    {
        List<Sprite> equalSprites = new List<Sprite>();

        long key = GetSpriteKey(sprite);

        if (map.TryGetValue(key, out List<Sprite> possibleSprites))
        {
            foreach (var spriteIt in possibleSprites)
            {
                if (IsEqualTextures(sprite, spriteIt))
                {
                    equalSprites.Add(spriteIt);
                }
            }
        }

        if (equalSprites.Count == 0)
        {
            Debug.Log("No one equal sprite found");
            return;
        }

        StringBuilder equalSpritesBuilder = new StringBuilder("Equal sprites:\n");

        foreach (var equalSpriteIt in equalSprites)
        {
            equalSpritesBuilder.Append(
                equalSpriteIt.name + $" Hash: {equalSpriteIt.texture.GetRawTextureData()}" + '\n');
        }

        Debug.Log(equalSpritesBuilder.ToString());
    }

    [Button]
    private void Check3()
    {
        List<Sprite> equalSprites = new List<Sprite>();

        long key = GetTextureKey(texture);

        if (map.TryGetValue(key, out List<Sprite> possibleSprites))
        {
            foreach (var spriteIt in possibleSprites)
            {
                if (IsEqualTextures(texture, spriteIt))
                {
                    equalSprites.Add(spriteIt);
                }
            }
        }

        if (equalSprites.Count == 0)
        {
            Debug.Log("No one equal sprite found");
            return;
        }

        StringBuilder equalSpritesBuilder = new StringBuilder("Equal sprites:\n");

        foreach (var equalSpriteIt in equalSprites)
        {
            equalSpritesBuilder.Append(
                equalSpriteIt.name + $" Hash: {equalSpriteIt.texture.GetRawTextureData()}" + '\n');
        }

        Debug.Log(equalSpritesBuilder.ToString());
    }

    private async Task<List<Sprite>> GetEqualSprites(int beginIndex, int endIndex)
    {
        List<Sprite> equalSprites = new List<Sprite>();

        if (beginIndex > endIndex || endIndex > sprites.Count)
        {
            return equalSprites;
        }

        for (var index = beginIndex; index < endIndex; index++)
        {
            var spriteIt = sprites[index];

            if (IsEqualTextures(sprite, spriteIt))
            {
                equalSprites.Add(spriteIt);
            }
        }

        return equalSprites;
    }

    public async Task<List<Sprite>> GetEqualSprites()
    {
        const int THREADS = 1;

        List<Sprite> equalSprites = new List<Sprite>();

        List<Task<List<Sprite>>> tasks = new List<Task<List<Sprite>>>();

        int step = sprites.Count / THREADS;
        int beginIndex = 0;

        if (step == 0)
            return equalSprites;

        while (beginIndex < sprites.Count)
        {
            int lastIndex = Mathf.Clamp(beginIndex + step, beginIndex, sprites.Count);
            var task = GetEqualSprites(beginIndex, lastIndex);

            tasks.Add(task);
            beginIndex += step;
        }

        Task.WaitAll(tasks.ToArray());

        tasks.ForEach(task => equalSprites.AddRange(task.Result));

        return equalSprites;
    }

    private bool IsEqualTextures(Sprite sprite1, Sprite sprite2)
    {
        byte[] rawTextureData1 = sprite1.texture.GetRawTextureData();
        byte[] rawTextureData2 = sprite2.texture.GetRawTextureData();

        return rawTextureData1.SequenceEqual(rawTextureData2);
    }

    private bool IsEqualTextures(Texture2D texture, Sprite sprite)
    {
        byte[] rawTextureData1 = texture.GetRawTextureData();
        byte[] rawTextureData2 = sprite.texture.GetRawTextureData();

        return rawTextureData1.SequenceEqual(rawTextureData2);
    }
}