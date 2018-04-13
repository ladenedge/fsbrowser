﻿
var appRoot = '/fsbrowser/';

function Item(data) {
    this.name = data.Name;
    this.fullPath = data.Path;
    this.mimeType = data.MimeType;
    this.isDir = data.MimeType === "inode/directory";
    this.size = formatSize(data.Size, this.isDir);
    this.mtime = formatTime(data.MTime);
    this.readOnly = data.ReadOnly;
    this.href = data.Self;
    this.hrefParent = data.Parent;
    this.hrefChildren = data.Children;
    this.children = ko.observableArray([]);
    this.icon = this.isDir ? '📂 ' : '📄 ';
    this.isRoot = this.fullPath.match(/\w:\\?$/);

    this.fetchChildren = function () {
        $.getJSON(this.hrefChildren, rsp => {
            var items = $.map(rsp, i => new Item(i, this))
                .sort((a, b) => b.isDir - a.isDir || a.name.localeCompare(b.name));
            this.children(items);
        });
    };

    this.parentName = () => {
        var parts = this.fullPath.split('\\').filter(p => p);
        parts.pop();
        return parts.pop();
    };

    this.parentPath = () => {
        var parts = this.fullPath.split('\\').filter(p => p);
        parts.pop();
        return parts.join('\\');
    };
}

function FileListViewModel() {
    this.currentDir = ko.observable(null);
    this.clipboard = ko.observable(null);
    this.clipboardOp = ko.observable(null);

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
        console.log('Fetching single item: ' + uri)
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

    this.paste = () => {

    };

    $(window).on("popstate", e => {
        console.log(`Popping '${e.originalEvent.state}' off stack`);
        this.fetchItem(e.originalEvent.state, false);
    });

    this.fetchItem(location.hash, true)
        .done(rsp => ko.applyBindings(this));
}

var formatTime = time => {
    if (!time) return null;
    var date = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
    return date.toLocaleString("en-US");
};

var formatSize = (size, isDir) => {
    // thank you, https://stackoverflow.com/a/18650828/222481
    if (isDir) return '';
    if (size === 0) return '0 Bytes';
    var units = ['Bytes', 'KB', 'MB', 'GB'];
    var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
    return Math.round(size / Math.pow(1024, i), 2) + ' ' + units[i];
};

var Model = new FileListViewModel();
