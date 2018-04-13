
var appRoot = '/fsbrowser/';

function FileListViewModel() {
    this.currentDir = ko.observable(null);
    this.clipboard = ko.observable(null);

    this.changeDir = (item, shouldPush) => {
        if (shouldPush) {
            console.log(`Pushing '${item.name}' onto stack`);
            window.history.pushState(item.fullPath, item.name, '#' + item.fullPath);
        }
        this.currentDir(item);
        return item.fetchChildren();
    };

    this.fetchItem = (path, shouldPush) => {
        if (path && path.charAt(0) === '#')
            path = path.substr(1);

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

    $(window).on("popstate", e => {
        if (!e.originalEvent.state) return;
        console.log(`Popping '${e.originalEvent.state}' off stack`);
        this.fetchItem(e.originalEvent.state, false);
    });

    this.fetchItem(location.hash, true)
        .done(rsp => ko.applyBindings(this));
}

var Model = new FileListViewModel();
