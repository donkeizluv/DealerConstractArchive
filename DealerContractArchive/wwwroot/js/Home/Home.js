//innit router
var router = new VueRouter({
    mode: 'history',
    base: 'Home',
    //root: window.location.href,
    routes: [
        { name: 'Default', path: '/', component: ContractsListing },
        { name: 'Index', path: '/Index', component: ContractsListing } //retuns default page = 1
        //{ name: 'Login', path: '/Login', component: Login }
        //{ path: '/:page/:type/:contains', component: ContractsListing }
    ]
});
//resgister
Vue.use(VueRouter);
Vue.use(VeeValidate);

//start vue instance
var vm = new Vue({
    el: "#app",
    router: router,
    mounted: function () {
        //console.log("app mounted");
        this.$data.role = getCookie("role");
        //if (this.$data.role != "ReadOnly")
        //    this.$data.IsReadOnly = false;
    },
    data: {
        role: ""
    },
    computed: {
        IsReadOnly: function () {
            if (this.$data.role == "ReadOnly")
                return true;
            return false;
        }
    }
});
