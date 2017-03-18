% initial values here:
l1 = 10;
l2 = 8;
position1 = 12
position2 = 12
position3 = 0;
rotation1 = 0;
rotation2 = 0;
rotation3 = 0;

% Step 1: create the DH parameter table
% unknowns: rotation1 and rotation2
theta1 = rotation1;
alpha1 = 0;
r1 = 0;
d1 = 0;

theta2 = rotation2 - pi/4;
alpha2 = 0;
r2 = l1;
d2 = 0;

theta3 = 0;
alpha3 = 0;
r3 = l2;
d3 = 0;

theta4 = 0;
alpha4 = 0;
r4 = l2;
d4 = 0;

theta5 = 0;
alpha5 = 0;
r5 = l2;
d5 = 0;

theta6 = 0;
alpha6 = 0;
r6 = l2;
d6 = 0;

% Step 2: make transformation matrix
T0_1 = createZmatrix(theta1,d1) * createXmatrix(alpha1,r1);
T1_2 = createZmatrix(theta2,d2) * createXmatrix(alpha2,r2);
T2_3 = createZmatrix(theta3,d3) * createXmatrix(alpha3,r3);
T3_4 = createZmatrix(theta4,d4) * createXmatrix(alpha4,r4);
T4_5 = createZmatrix(theta5,d5) * createXmatrix(alpha5,r5);
T5_6 = createZmatrix(theta6,d6) * createXmatrix(alpha6,r6);

% Step 3: create the overall transformation
T0_3 = T0_1 * T1_2 * T2_3 * T3_4 * T4_5 * T5_6;

% Step 4: calculate the actual overall transformation:
T0_G = [1,1,1,1;
        1,1,1,1;
        1,1,1,1;
        0,0,0,1];
    
theta

