namespace Torkjel.PFDS.Ch2;

using static System.Console;

/*
    Figure 2.3: Lst and ListOps implement the Stack/List abstraction in terms
    of a custom data type.

    Implementation notes:
    - C# does not (yet?) have sealed/closed type hierarchies, so the pattern
      matching is forced to match on _ instead of NIL.
    - Cons (the type) conflicts with Cons (the function), so using upper case
      for the type names.
    - Cons is overloaded, so that we can write a.Cons(b), instead of the
      more tedious a.Cons(b.Cons(Empty<T>))
    - This uses hierarchies of records and accompanying static classes
      containing extension functions to emulate structures in Standard ML.
      The use of static classes like this precludes using C# interfaces as an
      analogy to Standard ML signartures. If C# ever gets a notion of type-
      classes, that would fit in nicely here.
*/
public abstract record Lst<T> {

    // Define + operator for Lst concatenation. Neither :: or ++ are
    // overridable in C#
    public static Lst<T> operator +(Lst<T> xs, Lst<T> ys) => xs.Concat(ys);
}

public sealed record NIL<T> : Lst<T> {
    public override string ToString() => "Nil";
}

public sealed record CONS<T>(T head, Lst<T> tail) : Lst<T> {
    public T Head => head;
    public Lst<T> Tail => tail;
    public override string ToString() => $"({Head} :: {Tail})";
}

public class EmptyException : System.Exception { }

public static class ListOps {

    public static Lst<T> Empty<T>() => new NIL<T>();
    public static bool IsEmpty<T>(this Lst<T> lst) => lst == Empty<T>();
    public static Lst<T> Cons<T>(this T x, Lst<T> s) => new CONS<T>(x, s);
    public static Lst<T> Cons<T>(this T x, T y) => x.Cons(y.Cons(Empty<T>()));
    public static T Head<T>(this Lst<T> xs) => xs switch {
        CONS<T>(var x, _) => x,
        _ => throw new EmptyException() // Nil case
    };
    public static Lst<T> Tail<T>(this Lst<T> xs) => xs switch {
        CONS<T>(_, var xs_) => xs_,
        _=> throw new EmptyException() // Nil case
    };

    public static Lst<T> Concat<T>(this Lst<T> xs, Lst<T> ys) => xs switch {
        CONS<T>(var x, var xs_) => x.Cons(xs_ + ys),
        _ => ys // Nil case
    };

    public static Lst<T> Update<T>(this Lst<T> xs, int i, T y) => (xs, i) switch {
        (CONS<T> xs_, 0) => y.Cons(xs_.Tail),
        (CONS<T> xs_, _) => xs_.Head.Cons(xs_.Tail.Update(i - 1, y)),
        _ => throw new System.IndexOutOfRangeException()
    };

    public static Lst<Lst<T>> Suffixes<T>(this Lst<T> xs) => xs switch {
        CONS<T>(_, var xs_) => xs.Cons(xs_.Suffixes()),
        _ => Empty<Lst<T>>()
    };
}

public static class Run {

    public static void Main(string[] args) {
        WriteLine();
        ShowConcat();
        WriteLine();
        ShowUpdate();
        WriteLine();
        ShowSuffixes();
    }

    public static void ShowConcat() {
        WriteLine("Figure 2.5: Concatentating two lists");

        var xs = 0.Cons(1.Cons(2));
        var ys = 3.Cons(4.Cons(5));
        WriteLine("Before");
        WriteLine("xs : " + xs);
        WriteLine("ys : " + ys);

        var zs = xs + ys;
        WriteLine("After");
        WriteLine("zx : " + zs);
    }

    public static void ShowUpdate() {
        WriteLine("Figure 2.6: Update an element of a list");

        var xs = 0.Cons(1.Cons(2.Cons(3.Cons(4))));
        var ys = xs.Update(2, 7);
        WriteLine("Before:");
        WriteLine("xs : " + xs);
        WriteLine("After:");
        WriteLine("ys : " + ys);
    }

    public static void ShowSuffixes() {
        WriteLine("Exersice 2.1: Suffixes");

        var xs = 1.Cons(2.Cons(3.Cons(4)));
        var ys = xs.Suffixes();
        WriteLine("in : " + xs);
        WriteLine("out : " + ys);
    }
}