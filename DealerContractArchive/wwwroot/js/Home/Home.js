//innit router
var router = new VueRouter({
    mode: 'history',
    base: 'Home',
    //root: window.location.href,
    routes: [
        { name: 'Default', path: '/', component: DealerListing },
        { name: 'Index', path: '/Index', component: DealerListing } //retuns default page = 1
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
        this.$data.role = getCookie("role");
    },
    data: {
        role: "",
    },
    computed: {
        IsReadOnly: function () {
            if (this.$data.role == "ReadOnly")
                return true;
            return false;
        }
    }
});
