% T_0_G
% target transformation matrix relative to the
% base which defines the target position and orientation.

T_0_G = [1,1,1,1;
         1,1,1,1;
         1,1,1,1;
         0,0,0,1];
  
% dummy constants for now
l1 = 10;
l2 = 8;

alpha1 = 0; r1 = 0; d1 = 0;

alpha2 = 0; r2 = l1; d2 = 0;

alpha3 = 0; r3 = l2; d3 = 0;

alpha4 = 0; r4 = l2; d4 = 0;

alpha5 = 0; r5 = l2; d5 = 0;

alpha6 = 0; r6 = l2; d6 = 0;

% intermediate variables
Nko_0_6 = [T_0_G(1,3); T_0_G(2,3); T_0_G(3,3)];
Pko_4_6 = d6 .* Nko_0_6;
Pko_0_6 = [T_0_G(1,4); T_0_G(2,4); T_0_G(3,4)];
Pko_0_4 = Pko_0_6 - Pko_4_6;

% segment 1
theta1 = atan2(T_0_G(2,4) - d6*T_0_G(2,3), T_0_G(1,4) - d6*T_0_G(1,3));

% segment 3
% calculation here emitted so it is a dummy matrix
T_0_2 = [1,1,1,1;
         1,1,1,1;
         1,1,1,1;
         0,0,0,1];
Pko_0_2 = [T_0_2(1,4);T_0_2(2,4); T_0_2(3,4)];
Pko_2_4 = Pko_0_4 - Pko_0_2;
phi = asin((l1^2 - r2^2 + abs(Pko_2_4).^2) / (2*abs(Pko_2_4)*l1)) + ...
    asin((abs(Pko_2_4)-(l1^2-r2^2+abs(Pko_2_4).^2)) / r2);

alpha_ = atan2(-d4,r3);

theta3 = pi - phi - alpha_;

% segment 2
R_0_2 = T_0_2(1:3,1:3);
Pk2_2_4 = T_0_2(1:3,1:3) * Pko_2_4;
beta1 = atan2(Pk2_2_4(1,1),Pk2_2_4(2,1));
beta2 = asin(r2^2 - abs(Pko_2_4).^2 + l1^2) + ...
    asin((l1 - (r2^2 - abs(Pko_2_4).^2 + l1^2)/(2*l1)) / abs(Pko_2_4));
theta2 = pi/2 - abs(beta1) - beta2;

% segment 5
% calculation here emitted so it is a dummy matrix
T_0_4 = [1,1,1,1;
         1,1,1,1;
         1,1,1,1;
         0,0,0,1];
Nko_0_4 = [T_0_4(1,3);T_0_4(2,3);T_0_4(3,3)];
theta5 = pi - acos(Nko_0_4 * Nko_0_4');

% segment 6