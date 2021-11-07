export function debounce(delay: number = 600): MethodDecorator {
    return function (target: any, propertyKey: string, descriptor: PropertyDescriptor) {

        const original = descriptor.value;
        const key = `__timeout__${propertyKey}`;

        descriptor.value = function (...args) {
        clearTimeout(this[key]);
        this[key] = setTimeout(() => original.apply(this, args), delay);
        };

        return descriptor;
    };
}
