function T = createXmatrix (alpha,r)

T = zeros(4,4);
T(1,1) = 1;
T(1,4) = r;
T(2,2) = cos(alpha);
T(2,3) = -sin(alpha);
T(3,2) = sin(alpha);
T(3,3) = cos(alpha);
T(4,4) = 1;
