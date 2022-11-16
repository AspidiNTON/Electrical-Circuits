using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Main : MonoBehaviour
{
    Vector2 mousePrev = Vector2.zero;
    Vector2 mousePos;
    bool moving = false;
    bool isConnection;
    Connection movingConnection;
    Element movingElement;

    public Camera cam;
    public Texture2D[] textures;
    public List<Connection> connections = new List<Connection>();
    public List<Element> elements = new List<Element>();

    public void CreateResistor(float x, float y)
    {
        Element el = new Element();
        elements.Add(el);
        el.type = Type.Resistor;
        el.data = new float[1];
        el.data[0] = 10;
        el.gameObject.transform.position = new Vector3(x, y, 0);

        el.eCon = new Connection[2];
        connections.Add(new Connection(this, new Vector2(x - 1, y), el));
        el.eCon[0] = connections.Last();
        connections.Add(new Connection(this, new Vector2(x + 1, y), el));
        el.eCon[1] = connections.Last();

        el.gameObject.name = el.id + " resistor";
        el.spriteRenderer.sprite = Sprite.Create(textures[0], new Rect(0, 0, 200, 100), new Vector2(0.5f, 0.5f));
        el.boxCollider.size = new Vector2(2f, 1f);
    }

    public void CreateWire(Connection a, Connection b)
    {
        Element el = new Element();

        el.type = Type.Wire;
        el.gameObject.name = el.id + " wire";
        el.spriteRenderer.sprite = Sprite.Create(textures[1], new Rect(0, 0, 100, 50), new Vector2(0.5f, 0.5f));
        el.boxCollider.size = new Vector2(1f, 0.5f);

        el.eCon = new Connection[2];
        el.eCon[0] = a;
        a.AddElementToConnection(this, el);
        el.eCon[1] = b;
        b.AddElementToConnection(this, el);
        el.UpdateAngle();
        elements.Add(el);
    }

    private void Start()
    {
        CreateResistor(-4, 0);
        CreateResistor(0, 0);
        CreateResistor(4, 0);
        CreateResistor(0, -2);
        CreateWire(elements[0].eCon[1], elements[1].eCon[0]);
        CreateWire(elements[1].eCon[1], elements[2].eCon[0]);
        CreateWire(elements[0].eCon[1], elements[3].eCon[0]);
    }

    private void Update()
    {
        mousePos = new Vector2((Input.mousePosition.x / Screen.width * 32f / 9f - 16f / 9f) * cam.orthographicSize, (Input.mousePosition.y / Screen.height * 2 - 1) * cam.orthographicSize);
        if (Input.GetMouseButtonDown(0)) {
            List<Collider2D> res = new List<Collider2D>();
            ContactFilter2D fil = new ContactFilter2D();
            int con = 0, elem = 0;
            if (Physics2D.OverlapPoint(mousePos, fil, res) > 0)
            {
                for (int i = 0; i < res.Count; ++i)
                {
                    if (res[i].transform.gameObject.name.Split(new[] { ' ' })[1] == "connection")
                    {
                        con = int.Parse(res[i].transform.gameObject.name.Split(new[] { ' ' })[0]);
                    }
                    else
                    {
                        elem = int.Parse(res[i].transform.gameObject.name.Split(new[] { ' ' })[0]);
                    }
                }
                if (con != 0)
                {
                    isConnection = true;
                    for (int i = 0; i < connections.Count; ++i)
                    {
                        if (connections[i].id == con)
                        {
                            movingConnection = connections[i];
                            moving = true;
                            break;
                        }
                    }
                } else
                {
                    isConnection = false;
                    for (int i = 0; i < elements.Count; ++i)
                    {
                        if (elements[i].id == elem && elements[i].type != Type.Wire)
                        {
                            movingElement = elements[i];
                            moving = true;
                            break;
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            moving = false;
        }

        if (moving)
        {
            if (isConnection)
            {
                movingConnection.Move((mousePos - mousePrev).x, (mousePos - mousePrev).y);
            } else
            {
                movingElement.Move((mousePos - mousePrev).x, (mousePos - mousePrev).y);
            }
        }

        mousePrev = mousePos;
    }
}