using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class TowerTableData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string materialtype;
  public string Materialtype { get {return materialtype; } set { this.materialtype = value;} }
  
  [SerializeField]
  int rewardtype;
  public int Rewardtype { get {return rewardtype; } set { this.rewardtype = value;} }
  
  [SerializeField]
  int rewardvalue;
  public int Rewardvalue { get {return rewardvalue; } set { this.rewardvalue = value;} }
  
  [SerializeField]
  int spawnnum;
  public int Spawnnum { get {return spawnnum; } set { this.spawnnum = value;} }
  
  [SerializeField]
  float hp;
  public float Hp { get {return hp; } set { this.hp = value;} }
  
  [SerializeField]
  float attackpower;
  public float Attackpower { get {return attackpower; } set { this.attackpower = value;} }
  
  [SerializeField]
  float movespeed;
  public float Movespeed { get {return movespeed; } set { this.movespeed = value;} }
  
  [SerializeField]
  float knockbackpower;
  public float Knockbackpower { get {return knockbackpower; } set { this.knockbackpower = value;} }
  
}