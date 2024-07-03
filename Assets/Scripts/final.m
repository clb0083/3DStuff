
%attempt
clear;clc;


c=0.1;
m1=1;
m2=2;
t1=0;
tf=75;
dt=0.5;
t=[t1:dt:tf];

v1=-1;

v2=2;
n=1;
M=[v1,v2];
while t(n)<tf
    v1prime=(c*(v2(n)-v1(n)))/m1;
    v1(n+1)=v1(n)+v1prime*dt;

    v2prime=(-c*(v2(n)-v1(n)))/m2;
    v2(n+1)=v2(n)+v2prime*dt;

    n=n+1;
   
end

fprintf('The velocity eventually converges.\n')
fprintf('v1\t\t\tv2\n')
fprintf('%2.4f\t\t%2.4f\n',[v1;v2])