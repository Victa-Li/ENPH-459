function Z = createZmatrix (theta,d)

Z = zeros(4,4);
Z(1,1) = cos(theta);
Z(1,2) = -sin(theta);
Z(2,1) = sin(theta);
Z(2,2) = cos(theta);
Z(3,3) = 1;
Z(3,4) = d;
Z(4,4) = 1;