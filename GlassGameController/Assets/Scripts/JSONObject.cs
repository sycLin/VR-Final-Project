#define READABLE

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSONObject class
 * for use with Unity
 * Copyright Matt Schoen 2010
 */

public class JSONObject  {
	const int MAX_DEPTH = 1000;
	public enum Type { NULL, STRING, NUMBER, OBJECT, ARRAY, BOOL }
	public JSONObject parent;
	public Type type = Type.NULL;
	public List<object> list = new List<object>();
	public List<object> keys = new List<object>();
	public string str;
	public double n;
	public bool b;
	
	public static JSONObject nullJO { get { return new JSONObject(JSONObject.Type.NULL); } }
	public static JSONObject obj { get { return new JSONObject(JSONObject.Type.OBJECT); } }
	public static JSONObject arr { get { return new JSONObject(JSONObject.Type.ARRAY); } }
	
	public JSONObject(JSONObject.Type t) {
		type = t;
		switch(t) {
		case Type.ARRAY:
			list = new List<object>();
			break;
		case Type.OBJECT:
			list = new List<object>();
			keys = new List<object>();
			break;
		}
	}
	public JSONObject(bool b) {
		type = Type.BOOL;
		this.b = b;
	}
	public JSONObject(float f) {
		type = Type.NUMBER;
		this.n = f;
	}
	public JSONObject(Dictionary<string, string> dic) {
		type = Type.OBJECT;
		foreach(KeyValuePair<string, string> kvp in dic){
			keys.Add(kvp.Key);
			list.Add(kvp.Value);
		}
	}
	
	public JSONObject() { }
	
	public JSONObject(string str)
	{
		str = str.Replace("\\n", "");
		str = str.Replace("\\t", "");
		str = str.Replace("\\r", "");
		str = str.Replace("\t", "");
		str = str.Replace("\n", "");
		str = str.Replace("\r", "");
		str = str.Replace("\\", "");
		str = str.Replace(": ", ":");
		this.init(str, 0, str.Length);
	}
	
	public JSONObject(string str, int baseIndex, int length)
	{
		this.init(str, baseIndex, length);
	}
	
	public bool Equal(string str,int baseIndex, int length,string target) 
	{
		if (target.Length != length)
		{
			return false;
		}
		else 
		{
			for (int i = 0; i < target.Length; i++) 
			{
				if (target[i] != str[baseIndex + i]) 
				{
					return false;
				}
			}
			
			return true;
			
		}
	}
	
	public void init(string str,int baseIndex,int length) {	//create a new JSONObject from a string (this will also create any children, and parse the whole string)
		//Debug.Log(str);
		if(str != null) {
			
			
			// 
			
			if (length > 0)
			{
				
				
				if (Equal(str,baseIndex,length, "true"))
				{
					type = Type.BOOL;
					b = true;
				}
				else if (Equal(str, baseIndex, length, "false"))
				{
					type = Type.BOOL;
					b = false;
				}
				else if (Equal(str, baseIndex, length, "null"))
				{
					type = Type.NULL;
				}
				else if (str[baseIndex] == '"')
				{
					type = Type.STRING;
					this.str = str.Substring(baseIndex + 1, length - 2);
				} else {
					
					double result = 0;
					
					if (TryToParse(str,baseIndex,length, out result))
					{
						n = result;
						type = Type.NUMBER;
					} 
					else 
					{
						int token_tmp = 0;
						/*
						 * Checking for the following formatting (www.json.org)
						 * object - {"field1":value,"field2":value}
						 * array - [value,value,value]
						 * value - string	- "string"
						 *		 - number	- 0.0
						 *		 - bool		- true -or- false
						 *		 - null		- null
						 */
						switch (str[baseIndex])
						{
						case '{':
							type = Type.OBJECT;
							keys = new List<object>();
							list = new List<object>();
							break;
						case '[':
							type = JSONObject.Type.ARRAY;
							list = new List<object>();
							break;
						default:
							type = Type.NULL;
							//Debug.LogWarning("improper JSON formatting:" + str);
							return;
						}
						int depth = 0;
						bool openquote = false;
						bool inProp = false;
						for (int i = 1; i < length; i++)
						{
							
							char targetChar = str[baseIndex + i];
							
							if (targetChar == '\\')
							{
								i++;
								continue;
							}
							else if (targetChar == '"')
							{
								openquote = !openquote;
							}
							else if (targetChar == '[' || targetChar == '{')
							{
								depth++;
							}
							else if(depth == 0 && !openquote) {
								if (targetChar == ':' && !inProp) 
								{
									inProp = true;
									try {
										keys.Add(str.Substring(baseIndex + token_tmp + 2,  i - token_tmp - 3));
									} catch { /*Debug.Log(i + " - " + str.Length + " - " + str);*/ }
									token_tmp = i;
								}
								else if (targetChar == ',') 
								{
									inProp = false;
									list.Add(new JSONObject(str,baseIndex+token_tmp + 1, i - token_tmp - 1));
									token_tmp = i;
								}
								else if (targetChar == ']' || targetChar == '}')
								{
									list.Add(new JSONObject(str,baseIndex+token_tmp + 1, i - token_tmp - 1));
								}
							}
							else if (targetChar == ']' || targetChar == '}')
							{
								depth--;
							}
						}
					}
				}
			}
		} else {
			type = Type.NULL;	//If the string is missing, this is a null
		}
	}
	
	public bool TryToParse(string builder, int baseIndex, int length, out double result) 
	{
		if (length > 20)
		{
			result = 0;
			return false;
		}
		else 
		{
			string str = builder.Substring(baseIndex, length);
			
			bool success = double.TryParse(str,out result);
			
			return success;
		}
		
	}
	
	public void AddField(bool val) { Add(new JSONObject(val)); }
	public void AddField(float val) { Add(new JSONObject(val)); }
	public void AddField(int val) { Add(new JSONObject(val)); }
	
	public void RemoveField(string filed)
	{
		JSONObject obj = GetField(filed);
		if (obj != null)
		{		//Don't do anything if the object is null
			if (type != JSONObject.Type.OBJECT)
			{
				type = JSONObject.Type.OBJECT;		//Congratulations, son, you're an OBJECT now
				//Debug.LogWarning("tried to add a field to a non-object JSONObject.  We'll do it for you, but you might be doing something wrong.");
			}
			keys.Remove(filed);
			list.Remove(obj);
		}
		else
		{
			//Debug.Print("Remove Fail, No such Filed name!!!!!");
		}
	}
	
	public void Add(JSONObject obj) {
		if(obj != null) {		//Don't do anything if the object is null
			if(type != JSONObject.Type.ARRAY) {
				type = JSONObject.Type.ARRAY;		//Congratulations, son, you're an ARRAY now
				//Debug.LogWarning("tried to add an object to a non-array JSONObject.  We'll do it for you, but you might be doing something wrong.");
			}
			list.Add(obj);
		}
	}
	public void AddField(string name, bool val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, float val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, int val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, string val) {
		AddField(name, new JSONObject { type = JSONObject.Type.STRING, str = val });
	}
	public void AddField(string name, JSONObject obj) {
		if(obj != null){		//Don't do anything if the object is null
			if(type != JSONObject.Type.OBJECT){
				type = JSONObject.Type.OBJECT;		//Congratulations, son, you're an OBJECT now
				//Debug.LogWarning("tried to add a field to a non-object JSONObject.  We'll do it for you, but you might be doing something wrong.");
			}
			keys.Add(name);
			list.Add(obj);
		}
	}
	public void SetField(string name, JSONObject obj) {
		if(HasField(name)) {
			list.Remove(this[name]);
			keys.Remove(name);
		}
		AddField(name, obj);
	}
	public JSONObject GetField(string name) {
		if(type == JSONObject.Type.OBJECT)
			for(int i = 0; i < keys.Count; i++)
				if((string)keys[i] == name){
				if(i<list.Count){
					return (JSONObject)list[i];
				}
				else{
					return null;
				}
			}
		return null;
	}
	public bool HasField(string name) {
		if(type == JSONObject.Type.OBJECT)
			for(int i = 0; i < keys.Count; i++)
				if((string)keys[i] == name)
					return true;
		return false;
	}
	public void Clear() {
		type = JSONObject.Type.NULL;
		list.Clear();
		keys.Clear();
		str = "";
		n = 0;
		b = false;
	}
	public JSONObject Copy() {
		return new JSONObject(print());
	}
	/*
	 * The Merge function is experimental. Use at your own risk.
	 */
	public void Merge(JSONObject obj) {
		MergeRecur(this, obj);
	}
	static void MergeRecur(JSONObject left, JSONObject right) {
		if(right.type == JSONObject.Type.OBJECT) {
			for(int i = 0; i < right.list.Count; i++) {
				if(right.keys[i] != null) {
					string key = (string)right.keys[i];
					JSONObject val = (JSONObject)right.list[i];
					if(val.type == JSONObject.Type.ARRAY || val.type == JSONObject.Type.OBJECT) {
						if(left.HasField(key))
							MergeRecur(left[key], val);
						else
							left.AddField(key, val);
					} else {
						if(left.HasField(key))
							left.SetField(key, val);
						else
							left.AddField(key, val);
					}
				}
			}
		}// else left.list.Add(right.list);
	}
	
	
	public StringBuilder printByStringBuild()
	{
		return printByStringBuild(0);
	}
	
	public StringBuilder printByStringBuild(int depth) {	//Convert the JSONObject into a stiring
		if(depth++ > MAX_DEPTH) {
			//Debug.Log("reached max depth!");
			return new StringBuilder();
		}
		StringBuilder str =  new StringBuilder();
		switch(type) {
		case Type.STRING:
			str.Append("\"");
			str.Append(this.str);
			str.Append("\"");
			break;
		case Type.NUMBER:
			str.Append(n);
			
			break;
		case JSONObject.Type.OBJECT:
			if(list.Count > 0) {
				str.Append("{");
				#if(READABLE)	//for a bit more readability, comment the define above to save space
				str.Append( "\n");
				depth++;
				#endif
				for (int i = 0; i < keys.Count; i++)
				{
					string key = (string)keys[i];
					//check
					if(i>list.Count) return null;
					JSONObject obj = (JSONObject)list[i];
					if(obj != null) {
						#if(READABLE)
						for(int j = 0; j < depth; j++)
							str.Append("\t"); //for a bit more readability
						#endif
						str.Append("\""); str.Append(key);str.Append( "\":");
						str.Append(obj.printByStringBuild(depth).ToString()); str.Append( ",");
						#if(READABLE)
						str.Append("\n");
						#endif
					}
				}
				#if(READABLE)
				str.Remove(str.Length-1,1);
				//str = str.Substring(0, str.Length - 1);
				#endif
				str.Remove(str.Length-1,1);
				//str = str.Substring(0, str.Length - 1);
				str.Append("}");
			} else  str.Append("null");
			break;
		case JSONObject.Type.ARRAY:
			if(list.Count > 0) {
				str.Append("[");
				#if(READABLE)
				str.Append("\n"); //for a bit more readability
				depth++;
				#endif
				foreach(JSONObject obj in list) {
					if(obj != null) {
						#if(READABLE)
						for(int j = 0; j < depth; j++)
							str.Append("\t"); //for a bit more readability
						#endif
						str.Append(obj.printByStringBuild(depth).ToString());  str.Append(",");
						#if(READABLE)
						str.Append("\n"); //for a bit more readability
						#endif
					}
				}
				#if(READABLE)
				str.Remove(str.Length-1,1);
				//str = str.Substring(0, str.Length - 1);
				#endif
				str.Remove(str.Length-1,1);
				//str = str.Substring(0, str.Length - 1);
				str.Append("]");
			}
			break;
		case Type.BOOL:
			if(b)
				str.Append("true");
			else
				str.Append("false");
			break;
		case Type.NULL:
			str.Append("null");
			break;
		}
		return str;
	}
	
	
	
	
	public string print() {
		return print(0);
	}
	public string print(int depth) {	//Convert the JSONObject into a stiring
		if(depth++ > MAX_DEPTH) {
			//Debug.Log("reached max depth!");
			return "";
		}
		string str = "";
		switch(type) {
		case Type.STRING:
			str = "\"" + this.str + "\"";
			break;
		case Type.NUMBER:
			str += n;
			break;
		case JSONObject.Type.OBJECT:
			if(list.Count > 0) {
				str = "{";
				#if(READABLE)	//for a bit more readability, comment the define above to save space
				str += "\n";
				depth++;
				#endif
				for(int i = 0; i < list.Count; i++) {
					string key = (string)keys[i];
					JSONObject obj = (JSONObject)list[i];
					if(obj != null) {
						#if(READABLE)
						for(int j = 0; j < depth; j++)
							str += "\t"; //for a bit more readability
						#endif
						str += "\"" + key + "\":";
						str += obj.print(depth) + ",";
						#if(READABLE)
						str += "\n";
						#endif
					}
				}
				#if(READABLE)
				str = str.Substring(0, str.Length - 1);
				#endif
				str = str.Substring(0, str.Length - 1);
				str += "}";
			} else str += "null";
			break;
		case JSONObject.Type.ARRAY:
			if(list.Count > 0) {
				str = "[";
				#if(READABLE)
				str += "\n"; //for a bit more readability
				depth++;
				#endif
				foreach(JSONObject obj in list) {
					if(obj != null) {
						#if(READABLE)
						for(int j = 0; j < depth; j++)
							str += "\t"; //for a bit more readability
						#endif
						str += obj.print(depth) + ",";
						#if(READABLE)
						str += "\n"; //for a bit more readability
						#endif
					}
				}
				#if(READABLE)
				str = str.Substring(0, str.Length - 1);
				#endif
				str = str.Substring(0, str.Length - 1);
				str += "]";
			}
			break;
		case Type.BOOL:
			if(b)
				str += "true";
			else
				str += "false";
			break;
		case Type.NULL:
			str = "null";
			break;
		}
		return str;
	}
	public JSONObject this[int index] {
		get { return (JSONObject)list[index]; }
	}
	public JSONObject this[string index] {
		get { return GetField(index); }
	}
	public override string ToString() {
		return printByStringBuild().ToString();
	}
	public Dictionary<string, string> ToDictionary() {
		if (type == Type.OBJECT)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			for (int i = 0; i < list.Count; i++)
			{
				JSONObject val = (JSONObject)list[i];
				switch (val.type)
				{
				case Type.STRING: result.Add((string)keys[i], val.str); break;
				case Type.NUMBER: result.Add((string)keys[i], val.n + ""); break;
				case Type.BOOL: result.Add((string)keys[i], val.b + ""); break;
				default: /*Debug.LogWarning("Omitting object: " + (string)keys[i] + " in dictionary conversion");*/ break;
				}
			}
			return result;
		}
		else
		{
			//Debug.LogWarning("Tried to turn non-Object JSONObject into a dictionary");
		}
		return null;
	}
}