#include <iostream>
using namespace std;

struct Point {
	double x;
	double y;
	Point rotate(double angle) {
		Point P;
		P.x = x * cos(angle) + y * sin(angle);
		P.y = y * cos(angle) - x * sin(angle);
		return P;
	}
};

class Region {
public:
	Point Origin{};

	Region(Point O = { 0,0 })
		: Origin{ 0 }
	{}
	void translate(int x, int y) {
		Origin.x = Origin.x - x;
		Origin.y = Origin.y - y;
	}

	int orientation(Point p, Point q, Point r) {
		double val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

		if (val == 0) return 0;
		return (val > 0) ? 1 : -1;
	}
	bool onLine(Point A, Point P, Point B) {
		if (P.x <= max(A.x, B.x) && P.x >= min(A.x, B.x) && P.y <= max(A.y, B.y) && P.y >= min(A.y, B.y))
			return true;
		return false;
	}
	bool intersect(Point p1, Point q1, Point p2, Point q2) {
		int o1 = orientation(p1, q1, p2);
		int o2 = orientation(p1, q1, q2);
		int o3 = orientation(p2, q2, p1);
		int o4 = orientation(p2, q2, q1);
		if (o1 != o2 && o3 != o4)
			return true;
		if (o1 == 0 && onLine(p1, p2, q1)) return true;
		if (o2 == 0 && onLine(p1, q2, q1)) return true;
		if (o3 == 0 && onLine(p2, p1, q2)) return true;
		if (o4 == 0 && onLine(p2, q1, q2)) return true;
		return false;
	}
	

};

class Triangle : public Region {
public:
	Point A{};
	Point B{};
	Point C{};

	Triangle(Point A, Point B, Point C)
		:A{ A }, B{ B }, C{ C }
	{}

	bool contains(Point P) {
		P.x = P.x + Origin.x;
		P.y = P.y + Origin.y;
		Point extreme = { 10000, P.y };
		int count = 0, i = 0;
		if (intersect(A, B, P, extreme)) {
			if (orientation(A, P, B) == 0)
				return onLine(A, P, B);
			count++;
		}
		if (intersect(B, C, P, extreme)) {
			if (orientation(B, P, C) == 0)
				return onLine(B, P, C);
			count++;
		}
		if (intersect(C, A, P, extreme)) {
			if (orientation(C, P, A) == 0)
				return onLine(C, P, A);
			count++;
		}
		return count % 2;
	}
	Triangle rotate(double angle) {
		Triangle T{ A.rotate(angle), B.rotate(angle), C.rotate(angle) };
		return T;
	}
};

class Quadrilateral : public Region {
public:
	Point A{};
	Point B{};
	Point C{};
	Point D{};

	Quadrilateral(Point A, Point B, Point C, Point D)
		:A{ A }, B{ B }, C{ C }, D{ D }
	{}

	bool contains(Point p) {
		Point P;
		P.x = p.x + Origin.x;
		P.y = p.y + Origin.y;
		Point extreme = { 100000, P.y };
		int count = 0, i = 0;
		if (intersect(A, B, P, extreme)) {
			if (orientation(A, P, B) == 0)
				return onLine(A, P, B);
			count++;
		}
		if (intersect(B, C, P, extreme)) {
			if (orientation(B, P, C) == 0)
				return onLine(B, P, C);
			count++;
		}
		if (intersect(C, D, P, extreme)) {
			if (orientation(C, P, A) == 0)
				return onLine(C, P, A);
			count++;
		}
		if (intersect(A, D, P, extreme)) {
			if (orientation(A, P, D) == 0)
				return onLine(A, P, D);
			count++;
		}
		return count%2;
	}
	Quadrilateral rotate(double angle) {
		Quadrilateral Q{ A.rotate(-angle), B.rotate(-angle), C.rotate(-angle), D.rotate(-angle) };
		return Q;
	}
};

class Circle : public Region
{
public:
	Point center{};
	double radius{};

	Circle(Point c, double r)
		:center{ c }, radius{ r }
	{}
	bool contains(Point p) {
		double d = ((p.x - center.x + Origin.x) * (p.x - center.x + Origin.x)) + ((p.y - center.y + Origin.y) * (p.y - center.y + Origin.y));
		if (d <= radius * radius) return true;
		return false;
	}

	Circle rotate(double angle) {
		Circle C{ center.rotate(-angle), radius };
		return C;
	}
};

class CompCircle : public Region
{
public:
	Point center{};
	double radius{};

	CompCircle(Point c, double r)
		:center{ c }, radius{ r }
	{}
	bool contains(Point p) {
		double d = (p.x - center.x + Origin.x) * (p.x - center.x + Origin.x) + (p.y - center.y + Origin.y) * (p.y - center.y + Origin.y);
		if (d >= radius * radius) return true;
		return false;
	}
	CompCircle rotate(double angle) {
		CompCircle C{ center.rotate(-angle), radius };
		return C;
	}
};

class Annulus : public Region
{
public:
	Point center{};
	double r1{};
	double r2{};

	Annulus(Point c = { 0,0 }, double r1 = 1, double r2 = 2)
		:center{ c }, r1{ r1 }, r2{ r2 }
	{}
	bool contains(Point p) {
		double d = (p.x - center.x + Origin.x) * (p.x - center.x + Origin.x) + (p.y - center.y + Origin.y) * (p.y - center.y + Origin.y);
		if (d >= r1 * r1 && d <= r2 * r2) return true;
		return false;
	}
	Annulus rotate(double angle) {
		Annulus C{ center.rotate(-angle), r1, r2 };
		return C;
	}
};

class LShape : public Region
{
public:
	double a{};
	double b{};
	double c{};

	LShape(double a = 1, double b = 2, double c = 1)
		:a{ a }, b{ b }, c{ c }
	{}
	bool contains(Point p) {
		if (p.x + Origin.x <= a && p.x + Origin.x >= 0 && p.y + Origin.y <= b && p.y + Origin.y >= 0) return true;
		if (p.x + Origin.x <= c + a && p.x + Origin.x >= 0 && p.y + Origin.y <= a && p.y + Origin.y >= 0) return true;
		return false;
	}
};

int main() {
	//Triangle
	cout << "Triangle:\n";
	Triangle R{ {0,0}, {1,2}, {2,0} };
	Point P = { 1,1 };
	if (R.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 0.5,2 };
	if (R.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	Triangle R1 = R;
	R1.translate(1, 1);
	P = { 0.5,0.5 };
	if (R.contains(P) && !(R1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { 2,2 };
	if (R1.contains(P) && !(R.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	// Rotation
	P = { 1,1 };
	if (!(R.contains(P.rotate(1.57))) && (R.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { -1,1 };
	if ((R.contains(P.rotate(1.57))) && !(R.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	cout << "\n";

	// Quadrilateral
	cout << "Quadrilateral:\n";
	Quadrilateral Q{ {0,0}, {2,0}, {2,2}, {0,2} };
	P = { 1,1 };
	if (Q.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 0.5,3 };
	if (Q.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	Quadrilateral Q1 = Q;
	Q1.translate(1, 1);
	P = { 0.5,0.5 };
	if (Q.contains(P) && !(Q1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { 3,3 };
	if (Q1.contains(P) && !(Q.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	// Rotation
	P = { 1,0 };
	if (!(Q.contains(P.rotate(1.57))) && (Q.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { -1,1 };
	if ((Q.contains(P.rotate(1.57))) && !(Q.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	cout << "\n";

	// Circle
	cout << "Circle:\n";
	Circle C{ {0,0}, 3 };
	C.Origin = C.center;
	P = { 1,1 };
	if (C.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 4,0 };
	if (C.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	Circle C1 = C;
	C1.translate(1, 1);
	P = { 0,-3 };
	if (C.contains(P) && !(C1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { 1,4 };
	if (C1.contains(P) && !(C.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	cout << "\n";

	// Complement of a Circle
	cout << "CompCircle:\n";
	CompCircle CC{ {0,0}, 3 };
	CC.Origin = CC.center;
	P = { 4,0 };
	if (CC.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 0.5,0.5 };
	if (CC.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	CompCircle CC1 = CC;
	CC1.translate(1, 1);
	P = { 0,3 };
	if (CC.contains(P) && !(CC1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	// Rotation
	P = { 1,-2 };
	if (CC1.contains(P) && !(CC.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	cout << "\n";

	//Annulus
	cout << "Annulus:\n";
	Annulus A{ {0,0}, 3, 4 };
	A.Origin = A.center;
	P = { 3.5,0 };
	if (A.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 5,0 };
	if (A.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	Annulus A1 = A;
	A1.translate(1, 1);
	P = { 0,-3.5 };
	if (A.contains(P) && !(A1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	P = { 1,4.5 };
	if (A1.contains(P) && !(A.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	cout << "\n";

	//Lshape
	cout << "LShape:\n";
	LShape L{ 1,2,1 };
	P = { 1,1 };
	if (L.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	P = { 0.5,0.5 };
	if (L.contains(P)) cout << "Yes\n";
	else cout << "No\n";
	// Translation
	LShape L1 = L;
	L1.translate(1, 1);
	if (L.contains(P) && !(L1.contains(P))) cout << "Yes\n";
	else cout << "No\n";
	// Rotation
	if (L.contains(P.rotate(1.57))) cout << "Yes\n";
	else cout << "No\n";
}