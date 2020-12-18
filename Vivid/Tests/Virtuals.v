InheritantOne {
    foo()
}

InheritantOne VirtualTypeOne {
    foo() {
        println(1 + 2)
    }
}

execute_virtual_type_one() {
    x = VirtualTypeOne()

    x.foo()
    (x as InheritantOne).foo()
}

InheritantTwo {
    a: large

    bar()
}

InheritantTwo VirtualTypeTwo {
    b: decimal

    bar() {
        println(a * a + b * b)
    }
}

execute_virtual_type_two() {
    x = VirtualTypeTwo()
    x.a = 7
    x.b = 42

    x.bar()
    (x as InheritantTwo).bar()
}

InheritantThree {
    b: large

    baz(x: tiny, y: small): large
}

InheritantThree VirtualTypeThree {
    c: decimal

    baz(x: tiny, y: small) {
        if x > y {
            println(x)
            => x
        }
        else y > x {
            println(y)
            => y
        }
        else {
            println(c)
            => c
        }
    }
}

execute_virtual_type_three() {
    x = VirtualTypeThree()
    x.b = 1
    x.c = 10

    println(x.baz(1, -1))
    println((x as InheritantThree).baz(255, 32767))
    println((x as InheritantThree).baz(7, 7))
}

InheritantOne 
InheritantTwo 
InheritantThree 
VirtualTypeFour {
    foo() {
        a += 1
        b -= 1
    }

    bar() {
        a *= 7
        b *= 7
    }

    baz(x: tiny, y: small) {
        => a / b + x / y
    }
}

execute_virtual_type_four() {
    x = VirtualTypeFour()
    x.a = -6942
    x.b = 4269

    x.foo()
    x.bar()
    println(x.baz(64, 8)) # 7

    (x as InheritantOne).foo()
    (x as InheritantTwo).bar()
    println((x as InheritantThree).baz(0, 1)) # -1
}

init() {
    execute_virtual_type_one()
    execute_virtual_type_two()
    execute_virtual_type_three()
    execute_virtual_type_four()
    => 0
}