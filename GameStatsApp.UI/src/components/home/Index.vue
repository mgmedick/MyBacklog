<template>
    <div>
        <div v-if="!indexvm.isAuth">          
            <span>Not logged in page</span>
        </div>  
        <div v-else>
            <user-lists :indexvm="indexvm"></user-lists>
        </div>
    </div>    
</template>
<script>
    export default {
        name: "Index",
        props: {
            indexvm: Object
        },
        data: function () {
            return {
            };
        },
        watch: {},
        created: function () {
        },
        methods: {},
    };
</script>






