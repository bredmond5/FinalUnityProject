using UnityEngine;
using System.Collections;

public interface Timeable
{
    void begin();
    void reset();
    int enemiesLeft();
    void setParentManager(RoundManager manager); 
}