
var appRoot = '/fsbrowser/';

function FileListViewModel() {
    this.currentDir = ko.observable(null);
    this.clipboard = ko.observable(null);
    this.filter = ko.observable(null);
    this.recursive = ko.observable(false);

    this.currentKey = () => {
        // of the form: #<fullpath>(<filter>/g)
        var key = this.currentDir() && this.currentDir().fullPath;
        if (key && this.filter())
            key += `(${this.filter()}` + (this.recursive() ? '/g)' : ')');
        return key || '';
    };

    this.changeDir = (item, shouldPush) => {
        this.currentDir(item);
        if (shouldPush) {
            console.log(`Pushing '${this.currentKey()}' onto stack`);
            window.history.pushState(this.currentKey(), '', '#' + this.currentKey());
        }
        return item.fetchChildren(this.filter(), this.recursive());
    };

    this.fetchItem = (hashKey, shouldPush) => {
        // of the form: #<fullpath>(<filter>/g)
        hashKey = hashKey || '';
        var [s1, path, s2, filter, recursive] = hashKey.match(/#?([^#(]*)(\(([^\/]*)\/?(g)?\))?/);
        this.filter(filter);
        this.recursive(!!recursive);

        var uri = appRoot + "directories?path=" + (path || '');
        console.log('Fetching single item: ' + uri);

        return $.getJSON(uri, rsp => {
            var item = new Item(rsp);
            this.changeDir(item, shouldPush);
        });
    };

    this.delete = item => {
        if (!confirm(`Are you sure you want to delete this item?\n\n${item.fullPath}`))
            return;
        $.ajax({
            url: item.href,
            type: 'DELETE',
            success: rsp => this.currentDir().children.remove(item)
        });
    };

    this.cut = item => {
        this.clipboard({
            op: 'cut',
            item: item,
            parent: this.currentDir()
        });
    };

    this.copy = item => {
        this.clipboard({
            op: 'copy',
            item: item
        });
    };

    this.paste = () => {
        if (!this.clipboard()) return;

        var params = `&source=${this.clipboard().item.fullPath}&removeSource=${this.clipboard().op === 'cut'}`;
        $.ajax({
            url: this.currentDir().href + params,
            type: 'PUT',
            success: rsp => {
                this.currentDir().children.push(new Item(rsp));
                if (this.clipboard().parent)
                    this.clipboard().parent.children.remove(this.clipboard().item);
            }
        });
    };

    this.resetFilter = () => {
        this.filter(null);
        this.recursive(false);
        this.changeDir(this.currentDir(), true);
    };

    $(window).on("popstate", e => {
        if (!e.originalEvent.state) return;
        console.log(`Popping '${e.originalEvent.state}' off stack`);
        this.fetchItem(e.originalEvent.state, false);
    });

    this.fetchItem(location.hash, true)
        .done(rsp => ko.applyBindings(this));
}

ko.bindingHandlers.slideVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value));
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).slideDown(100) : $(element).slideUp(50);
    }
};
 
var Model = new FileListViewModel();
