%this function convert angle in unity to angle in robot arm control
%input : A (the angle around Z) B (the angle around X) C(the angle around Y) 
%(X is the X, Y is Z in unity, Z is Y in unity)
%output : AAX is the angle around Z in unity coordinate, AAY is the angle around X in unity coordinate
%AAZ is the angle around Y in unity coordinate
function [AAX,AAY,AAZ]=Robotarmtounity(AX,AY,AZ,X,Y,Z)
syms A B C
eqns = [X*cos(C)*cos(A)+Z*sin(A)*cos(C)+Y*sin(C)*cos(B)+X*sin(A)*sin(B)*sin(C)-Z*sin(B)*cos(A)*cos(C)==cos(AY)*(X*cos(AZ)+Y*sin(AZ))+Z*sin(AY),
    X*cos(A)*sin(C)+Z*sin(A)*sin(C)+Y*cos(C)*cos(B)+X*sin(A)*sin(B)*cos(C)-Z*sin(B)*cos(A)*cos(C)==cos(AX)*(X*sin(AZ)+Y*cos(AZ))-sin(AX)*(Z*cos(AY)-sin(AY)*(X*cos(AZ)+Y*sin(AZ))),
    Y*sin(B)-X*sin(A)*cos(B)+Z*cos(A)*cos(B)==sin(AX)*(X*sin(AZ)+Y*cos(AZ))-cos(AX)*(Z*cos(AY)-sin(AY)*(X*cos(AZ)+Y*sin(AZ)))];
S = solve(eqns, [A B C])
AAX=S.A
AAY=S.B
AAZ=S.C
end