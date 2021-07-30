using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot
{
    public GameObject bot;
    public Vector2 speed;
    public Vector2 position;

    public Bot(GameObject _bot, Vector2 _speed, Vector2 _pos)
    {
        bot = _bot;
        speed = _speed;
        position = _pos;
    }
}
