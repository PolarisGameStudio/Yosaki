using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class BossTableData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string stringid;
  public string Stringid { get {return stringid; } set { this.stringid = value;} }
  
  [SerializeField]
  string name;
  public string Name { get {return name; } set { this.name = value;} }
  
  [SerializeField]
  string title;
  public string Title { get {return title; } set { this.title = value;} }
  
  [SerializeField]
  string description;
  public string Description { get {return description; } set { this.description = value;} }
  
  [SerializeField]
  double hp;
  public double Hp { get {return hp; } set { this.hp = value;} }
  
  [SerializeField]
  double rewardminhp;
  public double Rewardminhp { get {return rewardminhp; } set { this.rewardminhp = value;} }
  
  [SerializeField]
  double rewardmaxhp;
  public double Rewardmaxhp { get {return rewardmaxhp; } set { this.rewardmaxhp = value;} }
  
  [SerializeField]
  float attackpowermin;
  public float Attackpowermin { get {return attackpowermin; } set { this.attackpowermin = value;} }
  
  [SerializeField]
  float attackpowermax;
  public float Attackpowermax { get {return attackpowermax; } set { this.attackpowermax = value;} }
  
  [SerializeField]
  bool islock;
  public bool Islock { get {return islock; } set { this.islock = value;} }
  
  [SerializeField]
  float[] rewardtypes = new float[0];
  public float[] Rewardtypes { get {return rewardtypes; } set { this.rewardtypes = value;} }
  
  [SerializeField]
  float[] rewardmaxvalues = new float[0];
  public float[] Rewardmaxvalues { get {return rewardmaxvalues; } set { this.rewardmaxvalues = value;} }
  
  [SerializeField]
  int abilitytype;
  public int Abilitytype { get {return abilitytype; } set { this.abilitytype = value;} }
  
  [SerializeField]
  float abilityvalue;
  public float Abilityvalue { get {return abilityvalue; } set { this.abilityvalue = value;} }
  
  [SerializeField]
  int maxlevel;
  public int Maxlevel { get {return maxlevel; } set { this.maxlevel = value;} }
  
  [SerializeField]
  string abilname;
  public string Abilname { get {return abilname; } set { this.abilname = value;} }
  
  [SerializeField]
  int upgradeprice;
  public int Upgradeprice { get {return upgradeprice; } set { this.upgradeprice = value;} }
  
  [SerializeField]
  float defense;
  public float Defense { get {return defense; } set { this.defense = value;} }
  
}