export function propertySorter(fields: string[]) {
    let dir: (1 | -1)[] = [];
    let i: number;
    let l = fields.length;
    
    fields = fields.map(function(o, i) {
        if (o[0] === "-") {
            dir[i] = -1;
            o = o.substring(1);
        } else {
            dir[i] = 1;
        }
        return o;
    });

    return function (a: object, b: object) {
        for (i = 0; i < l; i++) {
            const o = fields[i];
            if (a[o] > b[o]) return dir[i];
            if (a[o] < b[o]) return -(dir[i]);
        }
        return 0;
    };
}
