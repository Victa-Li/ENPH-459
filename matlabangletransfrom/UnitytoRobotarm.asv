%this function convert angle in unity to angle in robot arm control
%input : A (the angle around Z) B (the angle around X) C(the angle around Y) 
%(X is the X, Y is Z in unity, Z is Y in unity)
%output : AAX is the angle X, AAY is the angle Y, AAZ is the angle Z
function [AAX,AAY,AAZ]=UnitytoRobotarm(A,B,C,X,Y,Z)
syms AX AY AZ
eqns = [X*cos(C)*cos(A)+Z*sin(A)*cos(C)+Y*sin(C)*cos(B)+X*sin(A)*sin(B)*sin(C)-Z*sin(B)*cos(A)*cos(C)==cos(AY)*(X*cos(AZ)+Y*sin(AZ))+Z*sin(AY),
    X*cos(A)*sin(C)+Z*sin(A)*sin(C)+Y*cos(C)*cos(B)+X*sin(A)*sin(B)*cos(C)-Z*sin(B)*cos(A)*cos(C)==cos(AX)*(X*sin(AZ)+Y*cos(AZ))-sin(AX)*(Z*cos(AY)-sin(AY)*(X*cos(AZ)+Y*sin(AZ))),
    Y*sin(B)-X*sin(A)*cos(B)+Z*cos(A)*cos(B)==sin(AX)*(X*sin(AZ)+Y*cos(AZ))-cos(AX)*(Z*cos(AY)-sin(AY)*(X*cos(AZ)+Y*sin(AZ)))];
S = solve(eqns, [AX AY AZ])
AAX=S.AX
AAY=S.AY
AAZ=S.AZ
end