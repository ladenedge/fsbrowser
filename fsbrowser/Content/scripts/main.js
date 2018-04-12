
var appRoot = '/fsbrowser/';

function Item(data, parent) {
    this.parent = parent;
    this.name = ko.observable(data.Name);
    this.mimeType = data.MimeType;
    this.isRoot = data.Name == '/';
    this.isDir = ko.observable(data.MimeType === "inode/directory");
    this.href = data.Href;
    this._href = ko.observable(data.Href);
    this.icon = this.isDir() ? '📂 ' : '📄 ';
    this.relativePath = ko.observable(data.Href.replace(/^.*?filesystem\/root/, ''));
    this.fullPath = ko.observable(data.Path);
    this.children = ko.observableArray([]);

    var formatTime = time => {
        if (!time) return null;
        var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
        return date.toLocaleString("en-US");
    };
    var formatSize = size => {
        if (this.isDir()) return '';
        if (size == 0) return '0 Bytes';
        var units = ['Bytes', 'KB', 'MB', 'GB'];
        var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
        return Math.round(size / Math.pow(1024, i), 2) + ' ' + units[i];
    }
    this.size = ko.observable(formatSize(data.Size));
    this.mtime = formatTime(data.MTime);

    this.fetchChildren = function () {
        $.getJSON(this.href, rsp => {
            var items = $.map(rsp, i => new Item(i, this))
                .sort((a, b) => b.isDir() - a.isDir() || a.name().localeCompare(b.name()));
            this.children(items);
        });
    };
}

function FileListViewModel() {
    this.root = new Item({ Name: "/", MimeType: "inode/directory", Href: appRoot + "filesystem/root/" })
    this.currentDir = ko.observable(this.root);
    this.clipboard = ko.observable(null);

    this.changeDir = function (item) {
        window.history.pushState(item.fullPath(), item.name(), '#' + item.relativePath());
        this.currentDir(item);
        return item.fetchChildren();
    };

    this.changeDir(this.root);
}

ko.applyBindings(new FileListViewModel());
