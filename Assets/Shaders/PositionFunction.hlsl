#ifndef POSITIONFUNCTION_INCLUDE
#define POSITIONFUNCTION_INCLUDE

float3 PositionStash;

void DirectionUpdate_float(float3 Position, float Strength, float DeltaTime, out float3 Direction)
{
    float3 oldLoc = PositionStash;
    float3 newLoc = Position;
    
    oldLoc.x = lerp(oldLoc.x, newLoc.x, DeltaTime * Strength);
    oldLoc.y = lerp(oldLoc.y, newLoc.y, DeltaTime * Strength);
    oldLoc.z = lerp(oldLoc.z, newLoc.z, DeltaTime * Strength);
    
    PositionStash = oldLoc;
    Direction = oldLoc - newLoc;
}

#endif //POSITIONFUNCTION_INCLUDE

