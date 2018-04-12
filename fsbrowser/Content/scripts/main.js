
var appRoot = '/fsbrowser/';

function Item(data, parent) {
    this.parent = parent;
    this.name = ko.observable(data.Name);
    this.mimeType = data.MimeType;
    this.isRoot = data.Name === '/';
    this.isDir = ko.observable(data.MimeType === "inode/directory");
    this.href = data.HrefSelf;
    this.hrefChildren = data.HrefChildren;
    this.icon = this.isDir() ? '📂 ' : '📄 ';
    this.relativePath = ko.observable(data.HrefSelf.replace(/^.*?directories/, ''));
    this.fullPath = ko.observable(data.Path);
    this.children = ko.observableArray([]);

    var formatTime = time => {
        if (!time) return null;
        var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
        return date.toLocaleString("en-US");
    };
    var formatSize = size => {
        // thank you, https://stackoverflow.com/a/18650828/222481
        if (this.isDir()) return '';
        if (size === 0) return '0 Bytes';
        var units = ['Bytes', 'KB', 'MB', 'GB'];
        var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
        return Math.round(size / Math.pow(1024, i), 2) + ' ' + units[i];
    };
    this.size = ko.observable(formatSize(data.Size));
    this.mtime = formatTime(data.MTime);

    this.fetchChildren = function () {
        $.getJSON(this.hrefChildren, rsp => {
            var items = $.map(rsp, i => new Item(i, this))
                .sort((a, b) => b.isDir() - a.isDir() || a.name().localeCompare(b.name()));
            this.children(items);
        });
    };

    this.parentHref = () => {
        var parts = this.HrefSelf.split('/');
        parts.pop();
        return parts.join('/');
    };
}

function FileListViewModel() {
    this.root = new Item({ Name: "/", MimeType: "inode/directory", HrefChildren: appRoot + "directories" });
    this.currentDir = ko.observable(this.root);
    this.clipboard = ko.observable(null);

    this.changeDir = item => {
        window.history.pushState(item.fullPath(), item.name(), '#' + item.relativePath());
        this.currentDir(item);
        return item.fetchChildren();
    };

    this.fetchItem = path => {
        if (path === '' || path === '/')
            return this.root;
        $.getJSON(appRoot + "directories?path=" + path, rsp => {
            return new Item(rsp, this);
        });
    };

    this.changeDir(this.fetchItem(location.hash.trim('#')));
}

ko.applyBindings(new FileListViewModel());
