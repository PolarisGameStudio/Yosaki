using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class PassiveSkillData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string stringid;
  public string Stringid { get {return stringid; } set { this.stringid = value;} }
  
  [SerializeField]
  int requiremagicbookidx;
  public int Requiremagicbookidx { get {return requiremagicbookidx; } set { this.requiremagicbookidx = value;} }
  
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
  string skillname;
  public string Skillname { get {return skillname; } set { this.skillname = value;} }
  
}