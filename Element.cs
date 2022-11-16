using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element
{
	public Type type;
	public static int lastid = 0;
	public int id;
	public Connection[] eCon;
	public float[] data;
	public GameObject gameObject;
	public BoxCollider2D boxCollider;
	public SpriteRenderer spriteRenderer;
	public Element()
	{
		id = ++lastid;
		gameObject = new GameObject();
		spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		boxCollider = gameObject.AddComponent<BoxCollider2D>();
	}

	public void Move(float x, float y) //for non-wires
	{
		gameObject.transform.position += new Vector3(x, y, 0);
		for (int i = 0; i < eCon.Length; ++i)
		{
			eCon[i].gameObject.transform.position += new Vector3(x, y, 0);
			for (int j = 0; j < eCon[i].cElem.Count; ++j)
			{
				if (eCon[i].cElem[j].type == Type.Wire) eCon[i].cElem[j].UpdateAngle();
			}
		}
	}

	public void UpdateAngle() //for wires
    {
		Transform f = eCon[0].gameObject.transform;
		Transform s = eCon[1].gameObject.transform;
		gameObject.transform.position = (f.position + s.position) / 2;
		gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((f.position - s.position).y / (f.position - s.position).x) * Mathf.Rad2Deg);
		gameObject.transform.localScale = new Vector2(Vector2.Distance(f.position, s.position), 1);
	}

	/*public void SetRotation(float y) //for non-wires
	{
		float x = y - gameObject.transform.rotation.eulerAngles.z;

		gameObject.transform.rotation = Quaternion.Euler(0, 0, y);
	}*/
}

public class Connection
{
	public static int lastid = 0;
	public int id;
	public GameObject gameObject;
	public CircleCollider2D circleCollider;
	public SpriteRenderer spriteRenderer;
	public List<Element> cElem = new List<Element>();
	public Connection(Main main, Vector2 _pos, Element e)
	{
		id = ++lastid;
		gameObject = new GameObject();
		gameObject.transform.position = _pos;
		spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		circleCollider = gameObject.AddComponent<CircleCollider2D>();
		cElem.Add(e);
		gameObject.name = id + " connection";
		spriteRenderer.sprite = Sprite.Create(main.textures[2], new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
		circleCollider.radius = 0.25f;
	}

	public void Move(float x, float y)
	{
		gameObject.transform.position += new Vector3(x, y, 0);
		for (int i = 0; i < cElem.Count; ++i)
		{
			if (cElem[i].type == Type.Wire)
			{
				cElem[i].UpdateAngle();
			} else {
				cElem[i].gameObject.transform.position += new Vector3(x, y, 0);
				for (int j = 0; j < cElem[i].eCon.Length; ++j)
				{
					cElem[i].eCon[j].gameObject.transform.position += new Vector3(x, y, 0);
					for (int k = 0; k < cElem[i].eCon[j].cElem.Count; ++k)
					{
						if (cElem[i].eCon[j].cElem[k].type == Type.Wire) cElem[i].eCon[j].cElem[k].UpdateAngle();
					}
				}
				gameObject.transform.position -= new Vector3(x, y, 0);
			}
		}
	}

	public void AddElementToConnection(Main main, Element e)
    {
		cElem.Add(e);
		if (cElem.Count > 2) spriteRenderer.sprite = Sprite.Create(main.textures[3], new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
	}
}

public enum Type : sbyte
{
	Resistor,
	Wire
}